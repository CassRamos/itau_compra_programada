namespace Core.Interfaces
{
    public class MockCotacaoService : ICotacaoService
    {
        private readonly Dictionary<string, decimal> _precosMercado = new(StringComparer.OrdinalIgnoreCase)
        {
            { "PETR4", 28.50m },
            { "VALE3", 98.30m },
            { "ITUB4", 24.75m },
            { "BBDC4", 22.10m },
            { "ABEV3", 18.20m },
            { "WEGE3", 38.20m },
        };

        public Task<decimal> ObterPrecoAtualAsync(string ticker)
        {
            if (_precosMercado.TryGetValue(ticker, out var preco))
            {
                return Task.FromResult(preco);
            }

            return Task.FromResult(0m);
        }
    }
}
