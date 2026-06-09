namespace Veloco.DTOs.Booking;

public class RentalDetailDto
{
    public int Id { get; set; }
    public string VerificationDocument { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string? StripePaymentIntentId { get; set; }
}