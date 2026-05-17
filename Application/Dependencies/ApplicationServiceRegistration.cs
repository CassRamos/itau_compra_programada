using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Dependencies
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IClienteAppService, ClienteAppService>();
            services.AddScoped<ICestaAppService, CestaAppService>();
            services.AddScoped<IMotorComprasAppService, MotorComprasAppService>();
            services.AddScoped<IRebalanceamentoAppService, RebalanceamentoAppService>();

            return services;
        }
    }
}
