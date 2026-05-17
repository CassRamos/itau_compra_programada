namespace Core.Interfaces
{
    public interface IMessageBusService
    {
        Task PublicarAsync<T>(string topico, T mensagem);
    }
}
