namespace Application.DTOs
{
    public record ExecutarMotorRequest(DateTime DataReferencia);
    public record MotorComprasResponse(
        DateTime DataExecucao,
        int TotalClientes,
        decimal TotalConsolidado,
        List<OrdemCompraDTO> OrdensCompra,
        List<DistribuicaoDTO> Distribuicoes,
        List<ResiduoDTO> ResiduosCustMaster,
        int EventosIRPublicados,
        string Mensagem
        );

    public record OrdemCompraDTO(
        string ticker,
        int QuantidadeTotal,
        List<OrdemDetalheDTO> Detalhes,
        decimal PrecoUnitario,
        decimal ValorTotal
        );

    public record OrdemDetalheDTO(string Tipo, string Ticker, int Quantidade);

    public record DistribuicaoDTO(
        long ClienteId,
        string Nome,
        decimal ValorAporte,
        List<AtivoDistribuicaoDTO> Ativos
        );

    public record AtivoDistribuicaoDTO(string Ticker, int Quantidade);

    public record ResiduoDTO(string Ticker, int Quantidade);
}
