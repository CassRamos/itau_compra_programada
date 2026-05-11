using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    internal class ClienteRepository : IClienteRepository
    {

        private readonly AppDbContext _dbContext;

        public ClienteRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AdicionarAsync(Cliente cliente)
        {
            await _dbContext.Clientes.AddAsync(cliente);
        }

        public async Task<bool> CpfJaExistente(string cpf)
        {
            return await _dbContext.Clientes.AnyAsync(c => c.CPF == cpf);
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
