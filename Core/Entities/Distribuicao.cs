namespace Core.Entities
{
    public class Distribuicao
    {
        public long Id { get; private set; }
        public long OrdemCompraId { get; private set; }
        public long CustodiaFilhoteId { get; private set; }
        public string Ticker { get; private set; }
        public int Quantidade { get; private set; }
        public decimal PrecoUnitario { get; private set; }
        public DateTime DataDistribuicao { get; private set; }

        public OrdemCompra OrdemCompra { get; private set; }
        public Custodia CustodiaFilhote { get; private set; }

        protected Distribuicao() { }

        public Distribuicao(long ordemCompraId, long custodiaFilhoteId, string ticker, int quantidade, decimal precoUnitario)
        {
            this.OrdemCompraId = ordemCompraId;
            this.CustodiaFilhoteId = custodiaFilhoteId;
            this.Ticker = ticker.ToUpper();
            this.Quantidade = quantidade;
            this.PrecoUnitario = precoUnitario;
            this.DataDistribuicao = DateTime.Now;
        }
    }
}
