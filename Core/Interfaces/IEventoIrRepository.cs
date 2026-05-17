using Core.Entities;

namespace Core.Interfaces
{
    public interface IEventoIrRepository
    {
        Task AdicionarAsync(EventoIR evento);

    }
}
