using Core.Entities;
using Core.Interfaces;

namespace Application.Services
{
    public class MotorComprasAppService : IMotorComprasService
    {

        private readonly ICestaRepository _cestaRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IDistribuicaoRepository _distribuicaoRepository;
        private readonly IOrdemCompraRepository _ordemCompraRepository;
        private readonly ICotacaoService _cotacaoService;
        private readonly IUnitOfWork _unitOfWork;

        // In real life, this comes from configuration or database
        private const long CONTA_MASTER_ID = 1;
        public MotorComprasAppService(
            ICestaRepository cestaRepository,
            IClienteRepository clienteRepository,
            IDistribuicaoRepository distribuicaoRepository,
            IOrdemCompraRepository ordemCompraRepository,
            ICotacaoService cotacaoService,
            IUnitOfWork unitOfWork)
        {
            _cestaRepository = cestaRepository;
            _clienteRepository = clienteRepository;
            _distribuicaoRepository = distribuicaoRepository;
            _ordemCompraRepository = ordemCompraRepository;
            _cotacaoService = cotacaoService;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecutarMotorService()
        {
            var cestaAtiva = await _cestaRepository.ObterCestaAtivaAsync();
            if (cestaAtiva is null || !cestaAtiva.Itens.Any())
                throw new InvalidOperationException("Nenhuma cesta ativa encontrada para execução do motor de compras.");

            var clientesAtivos = await _clienteRepository.ObterClientesAtivos();
            if (!clientesAtivos.Any()) return;

            var volumeTotalPorTicker = new Dictionary<string, decimal>();
            foreach (var item in cestaAtiva.Itens)
            {
                var volumeParaEsteTicker = clientesAtivos.Sum(c => c.ValorMensal * (item.Percentual) / 100m);
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

                var quantidadeTotalComprar = (int)(volumeTotal / precoAtual);

                if (quantidadeTotalComprar > 0)
                {
                    var ordem = new OrdemCompra(CONTA_MASTER_ID, ticker, quantidadeTotalComprar, precoAtual, "FRACIONARIO");
                    _ordemCompraRepository.Adicionar(ordem);
                    ordensMaster.Add(ticker, ordem);
                }
            }

            await _unitOfWork.CommitAsync();

            foreach (var cliente in clientesAtivos)
            {
                if (cliente.ContaGrafica is null) continue;

                foreach (var item in cestaAtiva.Itens)
                {
                    var ticker = item.Ticker;
                    var precoAtual = cotacoesAtuais[ticker];

                    var volumeCliente = cliente.ValorMensal * (item.Percentual / 100m);
                    var quantidadeFracao = (int)(volumeCliente / precoAtual);

                    if (quantidadeFracao > 0 && ordensMaster.ContainsKey(ticker))
                    {
                        var ordemMaster = ordensMaster[ticker];

                        var custodia = cliente.ContaGrafica.Custodias.FirstOrDefault(c => c.Ticker == ticker);

                        if (custodia is null)
                        {
                            custodia = new Custodia(cliente.ContaGrafica.Id, ticker, quantidadeFracao, precoAtual);
                            cliente.ContaGrafica.AdicionarCustodia(custodia);
                            
                            await _unitOfWork.CommitAsync();
                        }
                        else
                        {
                            custodia.AdicionarQuantidade(quantidadeFracao, precoAtual);
                        }

                        var distribuicao = new Distribuicao(ordemMaster.Id, custodia.Id, ticker, quantidadeFracao, precoAtual);

                        _distribuicaoRepository.Adicionar(distribuicao);
                    }
                }
            }
            await _unitOfWork.CommitAsync();
        }

    }
}
   