namespace Veloco.DTOs.Payment;

public class PaymentDto
{
    public int Id { get; set; }
    public int RentalDetailId { get; set; }
    public decimal Amount { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? DealershipCut { get; set; }
    public decimal? OwnerPayout { get; set; }
    public string Status { get; set; }
    public string StripePaymentId { get; set; }
    public DateTime CreatedAt { get; set; }
}