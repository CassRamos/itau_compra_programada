using Core.Entities;

namespace Core.Interfaces
{
    public interface ICestaRepository
    {
        Task AdicionarAsync(CestaRecomendacao cesta);
        Task<CestaRecomendacao?> ObterCestaAtivaAsync();
        Task DesativarCestaAtualAsync();
        Task SalvarAlteracoesAsync();
        Task<IEnumerable<CestaRecomendacao>> ObterHistoricoCestasAsync();
    }
}
