namespace Veloco.DTOs.Booking;

public class BookingDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CarId { get; set; }
    public string Status { get; set; }
    public string BookingType { get; set; }
    public DateTime CreatedAt { get; set; }
    public RentalDetailDto? RentalDetail { get; set; }
    public ConsultationDetailDto? ConsultationDetail { get; set; }
}