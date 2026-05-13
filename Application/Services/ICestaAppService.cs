using Application.DTOs;

namespace Application.Services
{
    public interface ICestaAppService
    {
        Task<CestaResponse> CadastrarCestaAsync(CestaRequest request);
        Task<CestaDetailResponse?> ObterCestaAtualAsync();
        Task<IEnumerable<CestaDetailResponse>> ObterHistoricoCestasAsync();
    }
}
