using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MotorCompraController : ControllerBase
    {
        private readonly IMotorComprasService _motorComprasService;

        public MotorCompraController(IMotorComprasService motorComprasService)
        {
            _motorComprasService = motorComprasService;
        }

        [HttpPost("motor/executar")]
        public async Task<IActionResult> ExecutarMotor()
        {
            try
            {
                await _motorComprasService.ExecutarMotorService();
                return Ok(new { mensagem = "Motor executado com sucesso! Ordens e distribuições criadas." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
