namespace Core.Entities
{
    public class OrdemCompra
    {
        public long Id { get; private set; }
        public long ContaMasterId { get; private set; }
        public string Ticker { get; private set; }
        public int Quantidade { get; private set; }
        public decimal PrecoUnitario { get; private set; }
        public string TipoMercado { get; private set; }
        public DateTime DataExecucao { get; private set; }

        protected OrdemCompra() { }

        public OrdemCompra(long ContaMasterId, string ticker, int quantidade, decimal precoUnitario, string tipoMercado)
        {
            this.ContaMasterId = ContaMasterId;
            this.Ticker = ticker.ToUpper();
            this.Quantidade = quantidade;
            this.PrecoUnitario = precoUnitario;
            this.TipoMercado = tipoMercado.ToUpper();
            this.DataExecucao = DateTime.Now;
        }
    }
}
