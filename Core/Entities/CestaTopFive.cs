namespace Core.Entities
{
    public class CestaTopFive
    {
        public long Id { get; private set; }
        public string Nome { get; private set; }
        public bool Ativa { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataDesativacao { get; private set; }

        public readonly List<ItemCesta> _itens = new List<ItemCesta>();
        public IReadOnlyCollection<ItemCesta> Itens => _itens.AsReadOnly();

        protected CestaTopFive() { }

        public CestaTopFive(string nome, List<ItemCesta> itens)
        {
            if (string.IsNullOrWhiteSpace(nome))

                throw new ArgumentException("O nome da cesta é obrigatório.");

            if (itens == null || itens.Count != 5)
                throw new ArgumentException("A cesta deve conter exatamente 5 itens.", "QUANTIDADE_ATIVOS_INVALIDA");

            if (itens.Sum(i => i.Percentual) != 100m)
                throw new ArgumentException("A soma dos percentuais deve ser exatamente 100%", "PERCENTUAIS_INVALIDOS");

            Nome = nome;
            _itens = itens;
            Ativa = true;
            DataCriacao = DateTime.Now;
        }

        public void Desativar()
        {
            Ativa = false;
            DataDesativacao = DateTime.Now;
        }
    }
}

