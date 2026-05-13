namespace Core.Entities
{
    public class Custodia
    {
        public long Id { get; private set; }
        public long ContaGraficaId { get; private set; }
        public string Ticker { get; private set; }
        public int Quantidade { get; private set; }
        public decimal PrecoMedio { get; private set; }
        public DateTime DataUltimaAtualizacao { get; private set; }

        public ContaGrafica ContaGrafica { get; private set; }

        protected Custodia() { }

        public Custodia(long ContaGraficaId, string Ticker, int Quantidade, decimal PrecoMedio)
        {
            this.ContaGraficaId = ContaGraficaId;
            this.Ticker = Ticker.ToUpper();
            this.Quantidade = Quantidade;
            this.PrecoMedio = PrecoMedio;
            this.DataUltimaAtualizacao = DateTime.Now;
        }
    }
}
