using Core.Entities;
using Core.Enums;

namespace Application.DTOs
{
    public record AdesaoRequest(string Nome, string CPF, string Email, decimal ValorMensal);
    public record AdesaoResponse(long ClienteId, string Nome, string CPF, string Email, decimal ValorMensal, bool Ativo, DateTime DataAdesao, ContaGraficaDto ContaGrafica);

    public record ContaGraficaDto (long ContaGraficaId, string NumeroConta, TipoConta Tipo, DateTime DataCriacao );

}
