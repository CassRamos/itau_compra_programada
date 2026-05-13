using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ICestaAppService _service;

        public AdminController(ICestaAppService service)
        {
            _service = service;
        }

        [HttpGet("cesta/atual")]
        public async Task<IActionResult> ObterCestaAtual()
        {
            var response = await _service.ObterCestaAtualAsync();

            if (response is null)
            {
                return NotFound(new { erro = "Nenhuma cesta de recomendação ativa no momento" });
            }

            return Ok(response);
        }

        [HttpGet("cesta/historico")]
        public async Task<IActionResult> ObterHistoricoCestas()
        {
            var response = await _service.ObterHistoricoCestasAsync();

            return Ok(response);
        }

        [HttpPost("cesta")]
        public async Task<IActionResult> CadastrarCesta([FromBody] CestaRequest request)
        {
            try
            {
                var response = await _service.CadastrarCestaAsync(request);

                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    erro = ex.Message,
                    codigo = ex.ParamName ?? "DADOS_INVALIDOS"
                });
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    erro = "Ocorreu um erro interno ao processar a requisição",
                    detalhe = ex.Message
                });
            }

        }
    }
}
