using Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class MockKafkaService : IMessageBusService
    {
        private readonly ILogger<MockKafkaService> _logger;
        public MockKafkaService(ILogger<MockKafkaService> logger)
        {
            _logger = logger;
        }

        public Task PublicarAsync<T>(string topico, T mensagem)
        {
            var json = JsonSerializer.Serialize(mensagem);

            _logger.LogInformation("\n[KAFKA - MOCK] Topico : {Topico}\nPayload {mensagem}\n", topico, mensagem);

            return Task.CompletedTask;
        }
    }
}
