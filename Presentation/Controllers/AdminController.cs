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
