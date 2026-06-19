namespace Veloco.DTOs.Payment;

public class PaymentIntentResponseDto
{
    public string ClientSecret { get; set; }
    public string PaymentIntentId { get; set; }
    public decimal Amount { get; set; }
}