using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CestaRepository : ICestaRepository
    {
        private readonly AppDbContext _dbContext;

        public CestaRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AdicionarAsync(CestaRecomendacao cesta)
        {
            await _dbContext.CestaTopFive.AddAsync(cesta);
        }


        public async Task<CestaRecomendacao?> ObterCestaAtivaAsync()
        {
            return await _dbContext.CestaTopFive
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.Ativa);
        }
        public async Task DesativarCestaAtualAsync()
        {
            var cestaAtiva = await ObterCestaAtivaAsync();

            if (cestaAtiva != null)
            {
                cestaAtiva.Desativar();
            }
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<CestaRecomendacao>> ObterHistoricoCestasAsync()
        {
            return await _dbContext.CestaTopFive
                .Include(c => c.Itens)
                .OrderByDescending(c => c.DataCriacao)
                .ToListAsync();
        }
    }
}
