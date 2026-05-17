using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class EventoIrRepository : IEventoIrRepository
    {
        private readonly AppDbContext _dbContext;

        public EventoIrRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AdicionarAsync(EventoIR evento)
        {
            await _dbContext.EventosIR.AddAsync(evento);
        }
    }
}
