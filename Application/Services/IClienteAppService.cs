using Application.DTOs;

namespace Application.Services
{
    public interface IClienteAppService
    {
        Task<AdesaoResponse> RealizarAdesaoAsync(AdesaoRequest request);
    }
}
