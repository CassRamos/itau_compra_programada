using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Dependencies
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection"))
                ));

            services.AddScoped<ICestaRepository, CestaRepository>();
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IDistribuicaoRepository, DistribuicaoRepository> ();
            services.AddScoped<IEventoIrRepository, EventoIrRepository>();
            services.AddScoped<IOrdemCompraRepository, OrdemCompraRepository>();
            services.AddScoped<IRebalanceamentoRepository, RebalanceamentoRepository>();
            services.AddScoped<ICotacaoService, MockCotacaoService>();
            services.AddScoped<IMessageBusService, MockKafkaService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            return services;
        }
    }
}
