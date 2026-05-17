using Core.Entities;

namespace Core.Interfaces
{
    public interface IRebalanceamentoRepository
    {
        Task AdicionarAsync(Rebalanceamento rebalanceamento);
    }
}
