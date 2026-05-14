using Core.Entities;

namespace Core.Interfaces
{
    public interface IClienteRepository
    {
        Task AdicionarAsync(Cliente cliente);
        Task<bool> CpfJaExistente(string cpf);
        Task<Cliente?> ObterPorIdAsync(long id);
        Task<IEnumerable<Cliente>> ObterClientesAtivos();
        Task SalvarAlteracoesAsync();
    }
}
