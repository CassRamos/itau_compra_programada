using Core.Entities;

namespace Core.Interfaces
{
    public interface ICestaRepository
    {
        Task AdicionarAsync(CestaTopFive cesta);
        Task<CestaTopFive?> ObterCestaAtivaAsync();
        Task DesativarCestaAtualAsync();
        Task SalvarAlteracoesAsync();
    }
}
