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

        public async Task<AlterarValorMensalResponse> AtualizarValorMensalAsync(long id, AlterarValorMensalRequest request)
        {
            var cliente = await _repository.ObterPorIdAsync(id)
                ?? throw new KeyNotFoundException("Cliente não encontrado");

            var valorAnterior = cliente.AlterarValorMensal(request.NovoValor);

            await _repository.SalvarAlteracoesAsync();

            return new AlterarValorMensalResponse(
                cliente.Id,
                cliente.Nome,
                valorAnterior,
                cliente.ValorMensal,
                DateTime.Now,
                "Valor mensal atualizado. O novo valor será considerado a partir da próxima data de compra");
        }

        public async Task<PosicaoClienteResponse?> ObterPosicaoAsync(long id)
        {
            throw new NotImplementedException();
            //var cliente = await _repository.ObterPorIdAsync(id);

            //if (cliente is null) return null;

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
