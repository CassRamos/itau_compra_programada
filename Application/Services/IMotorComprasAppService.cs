using Application.DTOs;

namespace Application.Services
{
    public interface IMotorComprasAppService
    {
        Task<MotorComprasResponse> ExecutarMotorService(ExecutarMotorRequest request);
    }
}
