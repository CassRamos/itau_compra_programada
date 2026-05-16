using Core.Enums;

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

            var hash = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            var numeroConta = $"FLH-{hash}";

            ContaGrafica = new ContaGrafica(numeroConta, TipoConta.Filhote);
        }

        public decimal AlterarValorMensal(decimal novoValor)
        {
            if (novoValor <= 0)
            {
                throw new ArgumentException("O valor mensal deve ser maior que zero.");
            }

            if (novoValor < 20)
            {
                throw new ArgumentException("O valor mensal mínimo é de R$ 20,00 ");
            }

            var valorAnterior = ValorMensal;

            ValorMensal = novoValor;

            return valorAnterior;
        }

        public void SolicitarSaida()
        {
            if (!Ativo)
                throw new InvalidOperationException("O cliente já está inativo do produto");
                
            Ativo = false;
        }


    }
}
