namespace Application.DTOs
{
    public record AlterarValorMensalRequest(decimal NovoValor);
    public record AlterarValorMensalResponse(
            long clienteId,
            string Nome,
            decimal ValorMensalAnterior,
            decimal ValorMensalNovo,
            DateTime DataAlteracao,
            string Mensagem
        );

    public record AtivoPosicaoResponse(string Ticker, decimal Quantidade);

    public record PosicaoClienteResponse(
        long ClienteId,
        string Nome,
        decimal ValorMensalAgendado,
        List<AtivoPosicaoResponse> Ativos
    );
}
