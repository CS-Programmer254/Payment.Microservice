using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayPal.Application.DTOs;
using PayPal.Application.services;

namespace PayPal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayPalPayoutController : ControllerBase
    {
        private readonly IPayPalService _payPalService;

        public PayPalPayoutController(IPayPalService payPalService)
        {
            _payPalService = payPalService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendPayout([FromBody] PayoutRequestDto request)
        {
            var result = await _payPalService.SendPayoutAsync(request);

            return Ok(result);
        }
    }
}
