namespace Veloco.DTOs.Payment;

public class ConfirmPaymentDto
{
    public int BookingId { get; set; }
    public string PaymentIntentId { get; set; }
}