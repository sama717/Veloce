namespace Veloco.Models;

public class ConsultationDetail
{
    public int Id { get; set; }
    public int DealershipId { get; set; }
    public int BookingId { get; set; }
    public DateTime PreferredDate { get; set; }
    public string? Notes { get; set; }

    public Booking Booking { get; set; } 
    public Dealership Dealership { get; set; } 
}
