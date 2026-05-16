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
            var cliente = await _repository.ObterPorIdAsync(id);

            if (cliente is null) return null;

            var ativos = cliente.ContaGrafica?.Custodias
                ?.Select(c => new AtivoPosicaoResponse(c.Ticker, c.Quantidade))
                .ToList() ?? new List<AtivoPosicaoResponse>();

            return new PosicaoClienteResponse(
                cliente.Id,
                cliente.Nome,
                cliente.ValorMensal,
                ativos
                );
        }

        public async Task<AdesaoResponse> RealizarAdesaoAsync(AdesaoRequest request)
        {
            if (await _repository.CpfJaExistente(request.CPF))
                throw new Exception("CPF ja cadastrado no sistema.");

            var cliente = new Cliente(request.Nome, request.CPF, request.Email, request.ValorMensal);

            await _repository.AdicionarAsync(cliente);
            await _repository.SalvarAlteracoesAsync();

            return new AdesaoResponse(
                cliente.Id,
                cliente.Nome,
                cliente.CPF,
                cliente.Email,
                cliente.ValorMensal,
                cliente.Ativo,
                cliente.DataAdesao,
                new ContaGraficaDto(
                    cliente.ContaGrafica.Id,
                    cliente.ContaGrafica.NumeroConta,
                    cliente.ContaGrafica.Tipo,
                    cliente.ContaGrafica.DataCriacao
                )
            );
        }

        public async Task<SaidaProdutoResponse> SairProdutoAsync(long id)
        {
            var cliente = await _repository.ObterPorIdAsync(id)
                ?? throw new KeyNotFoundException("Cliente não encontrado");

            cliente.SolicitarSaida();

            await _repository.SalvarAlteracoesAsync();

            return new SaidaProdutoResponse(
                cliente.Id,
                cliente.Nome,
                cliente.Ativo,
                DateTime.Now,
                "Adesão encerrada. Sua posição em custódia foi mantida.");
        }
    }
}
