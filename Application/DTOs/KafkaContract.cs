namespace Application.DTOs
{
    public record MensagemDedoDuroKafka(
        long ClienteId,
        string CPF,
        string Ticker,
        decimal ValorOperacao,
        decimal ValorIR,
        DateTime Data);
}
