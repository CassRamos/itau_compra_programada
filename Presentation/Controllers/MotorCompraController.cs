using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MotorCompraController : ControllerBase
    {
        private readonly IMotorComprasAppService _motorComprasService;

        public MotorCompraController(IMotorComprasAppService motorComprasService)
        {
            _motorComprasService = motorComprasService;
        }

        [HttpPost("motor/executar-compra")]
        public async Task<IActionResult> ExecutarMotor([FromBody] ExecutarMotorRequest request)
        {
            try
            {
                var response = await _motorComprasService.ExecutarMotorService(request);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
