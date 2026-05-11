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
    }
}
