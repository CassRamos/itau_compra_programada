using Application.DTOs;
using Core.Entities;
using Core.Interfaces;

namespace Application.Services
{
    internal class CestaAppService : ICestaAppService
    {
        private readonly ICestaRepository _repository;

        public CestaAppService(ICestaRepository repository)
        {
            _repository = repository;
        }

        public async Task<CestaResponse> CadastrarCestaAsync(CestaRequest request)
        {
            var cestaAtual = await _repository.ObterCestaAtivaAsync();
            bool rebalanceamentoDisparado = false;

            if (cestaAtual != null)
            {
                await _repository.DesativarCestaAtualAsync();
                rebalanceamentoDisparado = true;
            }

            var itensDominio = request.Itens
                .Select(i => new ItemCesta(i.Ticker, i.Percentual))
                .ToList();

            var novaCesta = new CestaTopFive(request.Nome, itensDominio);

            await _repository.AdicionarAsync(novaCesta);
            await _repository.SalvarAlteracoesAsync();

            string mensagem = rebalanceamentoDisparado
                ? "Cesta atualizada. Rebalanceamento disparado."
                : "Primeira cesta cadastrada com suceso";

            return new CestaResponse(
                novaCesta.Id,
                novaCesta.Nome,
                novaCesta.Ativa,
                novaCesta.DataCriacao,
                rebalanceamentoDisparado,
                mensagem
                );
        }
    }
}
