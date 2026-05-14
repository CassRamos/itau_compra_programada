namespace Core.Interfaces
{
    public interface ICotacaoService
    {
        Task<decimal> ObterPrecoAtualAsync(string ticker);
    }
}
