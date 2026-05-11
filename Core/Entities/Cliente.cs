namespace Core.Entities
{
    public class Cliente
    {
        public long Id { get; private set; }
        public string Nome { get; private set; }
        public string CPF { get; private set; }
        public string Email { get; private set; }
        public decimal ValorMensal { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime DataAdesao { get; private set; }

        // navigation property

        public ContaGrafica ContaGrafica { get; private set; }

        protected Cliente() { }

        public Cliente(string nome, string cpf, string email, decimal valorMensal)
        {
            Nome = nome;
            CPF = cpf;
            Email = email;
            ValorMensal = valorMensal;
            Ativo = true;
            DataAdesao = DateTime.UtcNow;
        }

        public void AlterarValorMensal(decimal novoValor)
        {
            if (novoValor < 100)
            {
                throw new ArgumentException("O valor mensal mínimo é de R$ 100,00 ");
            }

            ValorMensal = novoValor;
        }

        public void SolicitarSaida()
        {
            Ativo = false;
        }


    }
}
