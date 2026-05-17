using Application.DTOs;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;

namespace Application.Services
{
    public class RebalanceamentoAppService : IRebalanceamentoAppService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly ICestaRepository _cestaRepository;
        private readonly IRebalanceamentoRepository _rebalanceamentoRepository;
        private readonly IEventoIrRepository _eventoIrRepository;
        private readonly ICotacaoService _cotacaoService;
        private readonly IMessageBusService _messageBusService;
        private readonly IUnitOfWork _unitOfWork;


        public RebalanceamentoAppService(
            IClienteRepository clienteRepository,
            ICestaRepository cestaRepository,
            IRebalanceamentoRepository rebalanceamentoRepository,
            IEventoIrRepository eventoIrRepository,
            ICotacaoService cotacaoService,
            IMessageBusService messageBusService,
            IUnitOfWork unitOfWork)
        {
            _clienteRepository = clienteRepository;
            _cestaRepository = cestaRepository;
            _rebalanceamentoRepository = rebalanceamentoRepository;
            _eventoIrRepository = eventoIrRepository;
            _cotacaoService = cotacaoService;
            _messageBusService = messageBusService;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecutarRebalanceamentoPorMudancaAsync()
        {
            var cestaAtiva = await _cestaRepository.ObterCestaAtivaAsync();

            if (cestaAtiva is null || !cestaAtiva.Itens.Any()) return;

            var clientes = await _clienteRepository.ObterClientesAtivos();

            var cotacoesAtuais = new Dictionary<string, decimal>();

            foreach (var cliente in clientes)
            {
                if (cliente.ContaGrafica is null || !cliente.ContaGrafica.Custodias.Any()) continue;

                decimal caixaLivre = 0m;
                decimal totalVendasMes = 0m;
                decimal lucroVendasMes = 0m;
                decimal valorTotalCarteira = 0m;

                foreach (var custodia in cliente.ContaGrafica.Custodias)
                {
                    if (!cotacoesAtuais.ContainsKey(custodia.Ticker))
                        cotacoesAtuais[custodia.Ticker] = await _cotacaoService.ObterPrecoAtualAsync(custodia.Ticker);

                    valorTotalCarteira += custodia.Quantidade * cotacoesAtuais[custodia.Ticker];
                }

                foreach (var item in cestaAtiva.Itens)
                {
                    if (!cotacoesAtuais.ContainsKey(item.Ticker))
                        cotacoesAtuais[item.Ticker] = await _cotacaoService.ObterPrecoAtualAsync(item.Ticker);
                }

                var tickerNaCesta = cestaAtiva.Itens.Select(i => i.Ticker).ToList();
                var custodiasParaVender = cliente.ContaGrafica.Custodias.Where(c => !tickerNaCesta.Contains(c.Ticker) && c.Quantidade > 0).ToList();

                foreach (var custodia in custodiasParaVender)
                {
                    var precoAtual = cotacoesAtuais[custodia.Ticker];
                    var valorVenda = custodia.Quantidade * precoAtual;

                    var lucro = valorVenda - (custodia.Quantidade * custodia.PrecoMedio);

                    caixaLivre += valorVenda;
                    totalVendasMes += valorVenda;
                    if (lucro > 0) lucroVendasMes += lucro;

                    await _rebalanceamentoRepository.AdicionarAsync(new Rebalanceamento(
                         cliente.Id,
                         TipoRebalanceamento.MudancaCesta,
                         custodia.Ticker,
                         null,
                         valorVenda));

                    custodia.AdicionarQuantidade(-custodia.Quantidade, precoAtual);
                }

                var itensNovos = cestaAtiva.Itens.Where(i => !cliente.ContaGrafica.Custodias.Any(c => c.Ticker == i.Ticker)).ToList();
                var percentualTotalNovos = itensNovos.Sum(i => i.Percentual);

                if (caixaLivre > 0 && percentualTotalNovos > 0)
                {
                    foreach (var item in itensNovos)
                    {

                        var proporcao = item.Percentual / percentualTotalNovos;

                        var valorDestinado = caixaLivre * proporcao;

                        var precoAtual = cotacoesAtuais[item.Ticker];
                        var quantidadeComprar = (int)(valorDestinado / precoAtual);

                        if (quantidadeComprar > 0)
                        {
                            var valorCompra = quantidadeComprar * precoAtual;
                            await _rebalanceamentoRepository.AdicionarAsync(new Rebalanceamento(
                                   cliente.Id,
                                   TipoRebalanceamento.MudancaCesta,
                                   null,
                                   item.Ticker,
                                   valorCompra));

                            var novaCustodia = new Custodia(cliente.ContaGrafica.Id, item.Ticker, quantidadeComprar, precoAtual);
                            cliente.ContaGrafica.AdicionarCustodia(novaCustodia);
                        }
                    }
                }

                var itensMantidos = cestaAtiva.Itens.Where(i => cliente.ContaGrafica.Custodias.Any(c => c.Ticker == i.Ticker)).ToList();

                foreach (var item in itensMantidos)
                {
                    var custodia = cliente.ContaGrafica.Custodias.First(c => c.Ticker == item.Ticker);
                    var precoAtual = cotacoesAtuais[item.Ticker];

                    var valorAtual = custodia.Quantidade * precoAtual;
                    var valorAlvo = valorTotalCarteira * (item.Percentual / 100m);

                    if (valorAtual > valorAlvo)
                    {
                        var diferenca = valorAtual - valorAlvo;
                        var quantidadeVender = (int)(diferenca / precoAtual);

                        if (quantidadeVender > 0)
                        {
                            var valorVenda = quantidadeVender * precoAtual;
                            var lucro = valorVenda - (quantidadeVender * custodia.PrecoMedio);

                            totalVendasMes += valorVenda;

                            if (lucro > 0) lucroVendasMes += lucro;

                            await _rebalanceamentoRepository.AdicionarAsync(new Rebalanceamento(
                                cliente.Id,
                                TipoRebalanceamento.MudancaCesta,
                                item.Ticker,
                                null,
                                valorVenda));

                            custodia.AdicionarQuantidade(-quantidadeVender, precoAtual);
                        }
                    }

                    if (totalVendasMes > 20000m && lucroVendasMes > 0)
                    {
                        var valorIr = Math.Round(lucroVendasMes * 0.20m, 2);

                        var eventoIr = new EventoIR(cliente.Id, TipoEventoIR.IrVenda, lucroVendasMes, valorIr);

                        await _eventoIrRepository.AdicionarAsync(eventoIr);

                        var kafkaMessage = new MensagemDedoDuroKafka(
                            cliente.Id,
                            cliente.CPF,
                            "REBALANCEAMENTO",
                            lucroVendasMes,
                            valorIr,
                            DateTime.Now);

                        await _messageBusService.PublicarAsync("itau-impostos-venda", kafkaMessage);
                        eventoIr.MarcarComoPublicado();
                    }
                }

                await _unitOfWork.CommitAsync();
            }
        }

        public async Task ExecutarRebalanceamentoPorDesvioAsync(decimal limiarDesvio = 5m)
        {
            var cestaAtiva = await _cestaRepository.ObterCestaAtivaAsync();
            if (cestaAtiva == null || !cestaAtiva.Itens.Any()) return;

            var clientes = await _clienteRepository.ObterClientesAtivos();
            var cotacoesAtuais = new Dictionary<string, decimal>();

            foreach (var cliente in clientes)
            {
                if (cliente.ContaGrafica == null || !cliente.ContaGrafica.Custodias.Any()) continue;

                decimal valorTotalCarteira = 0m;
                decimal caixaLivre = 0m;
                decimal totalVendasMes = 0m;
                decimal lucroVendasMes = 0m;


                foreach (var custodia in cliente.ContaGrafica.Custodias)
                {
                    if (!cotacoesAtuais.ContainsKey(custodia.Ticker))
                        cotacoesAtuais[custodia.Ticker] = await _cotacaoService.ObterPrecoAtualAsync(custodia.Ticker);

                    valorTotalCarteira += custodia.Quantidade * cotacoesAtuais[custodia.Ticker];
                }

                bool requerRebalanceamento = false;


                foreach (var custodia in cliente.ContaGrafica.Custodias)
                {
                    var itemCesta = cestaAtiva.Itens.FirstOrDefault(i => i.Ticker == custodia.Ticker);
                    var percentualAlvo = itemCesta?.Percentual ?? 0m;

                    var valorAtualCustodia = custodia.Quantidade * cotacoesAtuais[custodia.Ticker];
                    var percentualReal = (valorAtualCustodia / valorTotalCarteira) * 100m;


                    if (Math.Abs(percentualReal - percentualAlvo) >= limiarDesvio)
                    {
                        requerRebalanceamento = true;
                        break;
                    }
                }


                if (!requerRebalanceamento) continue;

                foreach (var custodia in cliente.ContaGrafica.Custodias)
                {
                    var itemCesta = cestaAtiva.Itens.FirstOrDefault(i => i.Ticker == custodia.Ticker);
                    var percentualAlvo = itemCesta?.Percentual ?? 0m;
                    var precoAtual = cotacoesAtuais[custodia.Ticker];

                    var valorAtualCustodia = custodia.Quantidade * precoAtual;
                    var valorAlvo = valorTotalCarteira * (percentualAlvo / 100m);

                    if (valorAtualCustodia > valorAlvo)
                    {
                        var diferencaFinanceira = valorAtualCustodia - valorAlvo;
                        var quantidadeVender = (int)(diferencaFinanceira / precoAtual);

                        if (quantidadeVender > 0)
                        {
                            var valorVenda = quantidadeVender * precoAtual;
                            var lucro = valorVenda - (quantidadeVender * custodia.PrecoMedio);

                            caixaLivre += valorVenda;
                            totalVendasMes += valorVenda;
                            if (lucro > 0) lucroVendasMes += lucro;

                            await _rebalanceamentoRepository.AdicionarAsync(new Rebalanceamento(
                                 cliente.Id, TipoRebalanceamento.Desvio, custodia.Ticker, null, valorVenda));

                            custodia.AdicionarQuantidade(-quantidadeVender, precoAtual);
                        }
                    }
                }

                var deficits = new Dictionary<string, decimal>();
                decimal deficitTotal = 0m;


                foreach (var item in cestaAtiva.Itens)
                {
                    var custodia = cliente.ContaGrafica.Custodias.FirstOrDefault(c => c.Ticker == item.Ticker);
                    var quantidadeAtual = custodia?.Quantidade ?? 0;
                    var precoAtual = cotacoesAtuais[item.Ticker];

                    var valorAtualCustodia = quantidadeAtual * precoAtual;
                    var valorAlvo = valorTotalCarteira * (item.Percentual / 100m);

                    if (valorAtualCustodia < valorAlvo)
                    {
                        var deficit = valorAlvo - valorAtualCustodia;
                        deficits.Add(item.Ticker, deficit);
                        deficitTotal += deficit;
                    }
                }

                if (caixaLivre > 0 && deficitTotal > 0)
                {
                    foreach (var deficit in deficits)
                    {
                        var ticker = deficit.Key;
                        var proporcaoDoDeficit = deficit.Value / deficitTotal;
                        var dinheiroParaComprar = caixaLivre * proporcaoDoDeficit;

                        var precoAtual = cotacoesAtuais[ticker];
                        var quantidadeComprar = (int)(dinheiroParaComprar / precoAtual);

                        if (quantidadeComprar > 0)
                        {
                            var valorCompra = quantidadeComprar * precoAtual;

                            await _rebalanceamentoRepository.AdicionarAsync(new Rebalanceamento(
                                    cliente.Id, TipoRebalanceamento.Desvio, null, ticker, valorCompra));

                            var custodiaExistente = cliente.ContaGrafica.Custodias.FirstOrDefault(c => c.Ticker == ticker);
                            if (custodiaExistente != null)
                            {
                                custodiaExistente.AdicionarQuantidade(quantidadeComprar, precoAtual);
                            }
                            else
                            {
                                var novaCustodia = new Custodia(cliente.ContaGrafica.Id, ticker, quantidadeComprar, precoAtual);
                                cliente.ContaGrafica.AdicionarCustodia(novaCustodia);
                            }
                        }
                    }
                }

                if (totalVendasMes > 20000m && lucroVendasMes > 0)
                {
                    var valorIr = Math.Round(lucroVendasMes * 0.20m, 2);
                    var eventoIr = new EventoIR(cliente.Id, TipoEventoIR.IrVenda, lucroVendasMes, valorIr);

                    await _eventoIrRepository.AdicionarAsync(eventoIr);

                    var kafkaMessage = new MensagemDedoDuroKafka(
                        cliente.Id, cliente.CPF, "DESVIO", lucroVendasMes, valorIr, DateTime.UtcNow);

                    await _messageBusService.PublicarAsync("itau-impostos-venda", kafkaMessage);
                    eventoIr.MarcarComoPublicado();
                }
            }

            await _unitOfWork.CommitAsync();
        }
    }
}
