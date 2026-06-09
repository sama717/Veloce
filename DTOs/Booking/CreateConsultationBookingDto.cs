namespace Veloco.DTOs.Booking;

public class CreateConsultationBookingDto
{
    public int CarId { get; set; }
    public int DealershipId { get; set; }
    public DateTime PreferredDate { get; set; }
    public string? Notes { get; set; }
}