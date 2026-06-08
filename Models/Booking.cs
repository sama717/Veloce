using Veloco.Enums;

namespace Veloco.Models;

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CarId { get; set; }
    public string VerificationDocument { get; set; }
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    
    public Car Car { get; set; }
    public User User { get; set; }
    public ICollection<Payment> Payments { get; set; } =  new List<Payment>();
}