using Core.Enums;

namespace Core.Entities
{
    public class ContaGrafica
    {
        public long Id { get; private set; }
        public long ClienteId { get; private set; }
        public string NumeroConta { get; private set; }
        public TipoConta Tipo { get; private set; }
        public DateTime DataCriacao { get; private set; }

        // Navigation property
        public Cliente Cliente { get; private set; }

    }
}
