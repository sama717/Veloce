namespace Veloco.DTOs.Booking;

public class CreateRentalBookingDto
{
    public int CarId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string VerificationDocument { get; set; }
}