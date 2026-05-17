namespace Application.Services
{
    public interface IRebalanceamentoAppService
    {
        Task ExecutarRebalanceamentoPorMudancaAsync();
        Task ExecutarRebalanceamentoPorDesvioAsync(decimal limiarDesvio = 5m);
    }
}
