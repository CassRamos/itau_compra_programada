using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    internal class OrdemCompraRepository : IOrdemCompraRepository
    {
        private readonly AppDbContext _dbContext;

        public OrdemCompraRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Adicionar(OrdemCompra ordem)
        {
            _dbContext.OrdensCompra.Add(ordem);
        }
    }
}
