using Application.DTOs;

namespace Application.Services
{
    public interface IClienteAppService
    {
        Task<AdesaoResponse> RealizarAdesaoAsync(AdesaoRequest request);
        Task<AlterarValorMensalResponse> AtualizarValorMensalAsync(long id, AlterarValorMensalRequest request);
        Task<PosicaoClienteResponse?> ObterPosicaoAsync(long id);
        Task<SaidaProdutoResponse> SairProdutoAsync(long id);
    }
}
