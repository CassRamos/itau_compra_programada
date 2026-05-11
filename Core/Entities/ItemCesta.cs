namespace Core.Entities
{
    public class ItemCesta
    {
        public long Id { get; private set; }
        public long CestaId { get; private set; }
        public string Ticker { get; private set; }
        public decimal Percentual { get; private set; }

        public CestaTopFive Cesta { get; private set; }

        protected ItemCesta() { }

        public ItemCesta(string ticker, decimal percentual)
        {
            Ticker = ticker.ToUpper();
            Percentual = percentual;
        }
    }
}
