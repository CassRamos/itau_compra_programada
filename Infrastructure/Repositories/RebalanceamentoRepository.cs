using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class RebalanceamentoRepository : IRebalanceamentoRepository
    {
        private readonly AppDbContext _dbContext;

        public RebalanceamentoRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AdicionarAsync(Rebalanceamento rebalanceamento)
        {
            await _dbContext.Rebalanceamentos.AddAsync(rebalanceamento);
        }
    }
}
