namespace Application.DTOs
{
    public record ItemCestaRequest(string Ticker, decimal Percentual);
    public record CestaRequest(string Nome, List<ItemCestaRequest> Itens);
    public record ItemCestaResponse(string Ticker, decimal Percentual);
    public record CestaResponse(
        long CestaId,
        string Nome,
        bool Ativa,
        DateTime DataCriacao,
        bool RebalanceamentoDisparado,
        string Mensagem);
    public record CestaDetailResponse(
        long Id,
        string Nome,
        bool Ativa,
        DateTime DataCriacao,
        DateTime? DataDesativacao,
        List<ItemCestaResponse> Itens);
}
