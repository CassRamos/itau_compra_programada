using Application.DTOs;
using Core.Entities;
using Core.Interfaces;

namespace Application.Services
{
    internal class ClienteAppService : IClienteAppService
    {

        private readonly IClienteRepository _repository;

        public ClienteAppService(IClienteRepository repository)
        {
            _repository = repository;
        }

        public async Task<AdesaoResponse> RealizarAdesaoAsync(AdesaoRequest request)
        {
            if (await _repository.CpfJaExistente(request.Cpf))
            {
                throw new Exception("CPF ja cadastrado no sistema.");
            }

            var cliente = new Cliente(request.Nome, request.Cpf, request.Email, request.ValorMensal);

            await _repository.AdicionarAsync(cliente);
            await _repository.SalvarAlteracoesAsync();

            return new AdesaoResponse(cliente.Id, cliente.Nome, "Adesao realizada com sucesso.");
        }
    }
}
