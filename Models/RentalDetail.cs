namespace Veloco.Models;

public class RentalDetail
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string VerificationDocument { get; set; }
    public DateTime StartDate { get; set; }   
    public DateTime EndDate { get; set; }    
    public decimal TotalPrice { get; set; }
    public string? StripePaymentIntentId { get; set; } 
    
    public Booking Booking { get; set; }
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}