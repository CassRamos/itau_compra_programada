using Application.DTOs;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;

namespace Application.Services
{
    public class MotorComprasAppService : IMotorComprasAppService
    {

        private readonly ICestaRepository _cestaRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IDistribuicaoRepository _distribuicaoRepository;
        private readonly IEventoIrRepository _eventoIrRepository;
        private readonly IOrdemCompraRepository _ordemCompraRepository;
        private readonly ICotacaoService _cotacaoService;
        private readonly IMessageBusService _messageBusService;
        private readonly IUnitOfWork _unitOfWork;

        // In real life, this comes from configuration or database
        private const long CONTA_MASTER_ID = 1;
        public MotorComprasAppService(
            ICestaRepository cestaRepository,
            IClienteRepository clienteRepository,
            IDistribuicaoRepository distribuicaoRepository,
            IEventoIrRepository eventoIrRepository,
            IOrdemCompraRepository ordemCompraRepository,
            ICotacaoService cotacaoService,
            IMessageBusService messageBusService,
            IUnitOfWork unitOfWork)
        {
            _cestaRepository = cestaRepository;
            _clienteRepository = clienteRepository;
            _distribuicaoRepository = distribuicaoRepository;
            _eventoIrRepository = eventoIrRepository;
            _ordemCompraRepository = ordemCompraRepository;
            _cotacaoService = cotacaoService;
            _messageBusService = messageBusService;
            _unitOfWork = unitOfWork;
        }

        public async Task<MotorComprasResponse> ExecutarMotorService(ExecutarMotorRequest request)
        {
            var cestaAtiva = await _cestaRepository.ObterCestaAtivaAsync();
            if (cestaAtiva is null || !cestaAtiva.Itens.Any())
                throw new InvalidOperationException("Nenhuma cesta ativa encontrada para execução do motor de compras.");

            var clientesAtivos = await _clienteRepository.ObterClientesAtivos();
            if (!clientesAtivos.Any())
                throw new InvalidOperationException("Nenhum cliente ativo encontrado para execução do motor de compras.");

            decimal totalConsolidade = clientesAtivos.Sum(c => c.ValorMensal / 3m);
            int eventosIRPublicados = 0;
            var ordensResponse = new List<OrdemCompraDTO>();
            var distribuicoesResponse = new List<DistribuicaoDTO>();
            var totalDistribuidoPorTicker = new Dictionary<string, int>();

            var volumeTotalPorTicker = new Dictionary<string, decimal>();
            foreach (var item in cestaAtiva.Itens)
            {
                var volumeParaEsteTicker = clientesAtivos.Sum(c => (c.ValorMensal / 3m) * (item.Percentual) / 100m);
                volumeTotalPorTicker.Add(item.Ticker, volumeParaEsteTicker);
            }

            var ordensMaster = new Dictionary<string, OrdemCompra>();
            var cotacoesAtuais = new Dictionary<string, decimal>();

            foreach (var kvp in volumeTotalPorTicker)
            {
                var ticker = kvp.Key;
                var volumeTotal = kvp.Value;

                var precoAtual = await _cotacaoService.ObterPrecoAtualAsync(ticker);
                cotacoesAtuais.Add(ticker, precoAtual);
                totalDistribuidoPorTicker[ticker] = 0;

                var quantidadeTotalComprar = (int)(volumeTotal / precoAtual);

                if (quantidadeTotalComprar > 0)
                {
                    var ordem = new OrdemCompra(CONTA_MASTER_ID, ticker, quantidadeTotalComprar, precoAtual, "FRACIONARIO");
                    _ordemCompraRepository.Adicionar(ordem);
                    ordensMaster.Add(ticker, ordem);

                    ordensResponse.Add(new OrdemCompraDTO(
                        ticker,
                        quantidadeTotalComprar,
                        new List<OrdemDetalheDTO> { new OrdemDetalheDTO("FRACIONARIO", $"{ticker}F", quantidadeTotalComprar) },
                        precoAtual,
                        quantidadeTotalComprar * precoAtual
                        ));
                }
            }

            await _unitOfWork.CommitAsync();

            foreach (var cliente in clientesAtivos)
            {
                if (cliente.ContaGrafica is null) continue;

                var valorAporteCliente = cliente.ValorMensal / 3m;

                var distribuicaoCliente = new DistribuicaoDTO(cliente.Id, cliente.Nome, valorAporteCliente, new List<AtivoDistribuicaoDTO>());

                foreach (var item in cestaAtiva.Itens)
                {
                    var ticker = item.Ticker;
                    var precoAtual = cotacoesAtuais[ticker];

                    var volumeCliente = valorAporteCliente * (item.Percentual / 100m);
                    var quantidadeFracao = (int)(volumeCliente / cotacoesAtuais[item.Ticker]);

                    if (quantidadeFracao > 0 && ordensMaster.ContainsKey(item.Ticker))
                    {
                        var ordemMaster = ordensMaster[item.Ticker];

                        var custodia = cliente.ContaGrafica.Custodias.FirstOrDefault(c => c.Ticker == item.Ticker);

                        if (custodia is null)
                        {
                            custodia = new Custodia(cliente.ContaGrafica.Id, item.Ticker, quantidadeFracao, precoAtual);
                            cliente.ContaGrafica.AdicionarCustodia(custodia);
                        }
                        else
                        {
                            custodia.AdicionarQuantidade(quantidadeFracao, precoAtual);
                        }

                        var distribuicao = new Distribuicao(ordemMaster.Id, custodia, item.Ticker, quantidadeFracao, precoAtual);

                        _distribuicaoRepository.Adicionar(distribuicao);

                        var valorOperacao = quantidadeFracao * precoAtual;
                        var valorIr = Math.Round(valorOperacao * 0.00005m, 2);

                        if (valorIr > 0)
                        {
                            var eventoIr = new EventoIR(cliente.Id, TipoEventoIR.DedoDuro, valorOperacao, valorIr);
                            await _eventoIrRepository.AdicionarAsync(eventoIr);

                            var kafkaMessage = new MensagemDedoDuroKafka(
                                cliente.Id,
                                cliente.CPF,
                                item.Ticker,
                                valorOperacao,
                                valorIr,
                                DateTime.Now);

                            await _messageBusService.PublicarAsync("itau-impostos-dedoduro", kafkaMessage);

                            eventoIr.MarcarComoPublicado();
                            eventosIRPublicados++;
                        }

                        distribuicaoCliente.Ativos.Add(new AtivoDistribuicaoDTO(item.Ticker, quantidadeFracao));
                        totalDistribuidoPorTicker[item.Ticker] += quantidadeFracao;
                    }
                }

                if (distribuicaoCliente.Ativos.Any())
                {
                    distribuicoesResponse.Add(distribuicaoCliente);
                }
            }

            await _unitOfWork.CommitAsync();

            var residuosResponse = new List<ResiduoDTO>();
            foreach (var ordem in ordensMaster)
            {
                var ticker = ordem.Key;
                var sobraMaster = ordem.Value.Quantidade - totalDistribuidoPorTicker[ticker];

                if (sobraMaster > 0)
                    residuosResponse.Add(new ResiduoDTO(ticker, sobraMaster));
            }
            return new MotorComprasResponse(
                DateTime.UtcNow,
                clientesAtivos.Count(),
                totalConsolidade,
                ordensResponse,
                distribuicoesResponse,
                residuosResponse,
                eventosIRPublicados,
                $"Compra programada executada com sucesso para {clientesAtivos.Count()} clientes."
             );
        }
    }
}

