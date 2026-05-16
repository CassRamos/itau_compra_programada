using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteAppService _service;

        public ClientesController(IClienteAppService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPosicao(long id)
        {
            var posicao = await _service.ObterPosicaoAsync(id);
            if (posicao is null) return NotFound();
            return Ok(posicao);
        }

        [HttpPost("adesao")]
        public async Task<IActionResult> Adesao([FromBody] AdesaoRequest request)
        {
            try
            {
                var response = await _service.RealizarAdesaoAsync(request);
                return CreatedAtAction(nameof(Adesao), new { id = response.ClienteId }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message, codigo = "ERRO_ADESAO" });
            }
        }

        [HttpPut("{clienteId}/valor-mensal")]
        public async Task<IActionResult> AlterarValorMensal(long clienteId, [FromBody] AlterarValorMensalRequest request)
        {
            try
            {
                var response = await _service.AtualizarValorMensalAsync(clienteId, request);

                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { erro = "Cliente não encontrado.", codigo = "CLIENTE_NAO_ENCONTRADO" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { erro = ex.Message, codigo = ex.ParamName });
            }
        }
    }
}
