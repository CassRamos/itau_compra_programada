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

            var novaCesta = new CestaRecomendacao(request.Nome, itensDominio);

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

        public async Task<CestaDetailResponse?> ObterCestaAtualAsync()
        {
            var cesta = await _repository.ObterCestaAtivaAsync();

            if (cesta is null) return null;

            return MapToDetailResponse(cesta);
        }

        public async Task<IEnumerable<CestaDetailResponse>> ObterHistoricoCestasAsync()
        {
            var historico = await _repository.ObterHistoricoCestasAsync();

            return historico.Select(MapToDetailResponse);

        }

        private static CestaDetailResponse MapToDetailResponse(CestaRecomendacao cesta)
        {
            var itensDto = cesta.Itens
                .Select(i => new ItemCestaResponse(i.Ticker, i.Percentual))
                .ToList();

            return new CestaDetailResponse(
                cesta.Id,
                cesta.Nome,
                cesta.Ativa,
                cesta.DataCriacao,
                cesta.DataDesativacao,
                itensDto);
        }
    }
}
