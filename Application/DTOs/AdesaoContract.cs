namespace Application.DTOs
{
    public record AdesaoRequest(string Nome, string Cpf, string Email, decimal ValorMensal);
    public record AdesaoResponse(long ClienteId, string Nome, string Mensagem);

}
