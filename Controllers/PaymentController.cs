using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veloco.DTOs.Payment;
using Veloco.Interfaces;

namespace Veloce.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController(IPaymentService paymentService) : ControllerBase
{
    private readonly IPaymentService _paymentService = paymentService;
    
    [HttpPost("create-payment-intent")]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentDto dto)
    {
        var result = await _paymentService.CreatePaymentIntentAsync(dto.BookingId);
        return Ok(result);
    }

    [HttpPost("confirm-payment")]
    public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentDto dto)
    {
        var result = await _paymentService.ConfirmPaymentAsync(dto.BookingId, dto.PaymentIntentId);
        return Ok(new { success = result, message = "Payment confirmed successfully" });
    }
}