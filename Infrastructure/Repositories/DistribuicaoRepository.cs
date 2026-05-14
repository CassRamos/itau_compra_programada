using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class DistribuicaoRepository : IDistribuicaoRepository
    {
        private readonly AppDbContext _dbContext;

        public DistribuicaoRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Adicionar(Distribuicao distribuicao)
        {
            _dbContext.Distribuicoes.Add(distribuicao);
        }
    }
}
