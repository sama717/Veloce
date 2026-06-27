using Veloco.Enums;

namespace Veloco.Models;

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CarId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public BookingStatus Status { get; set; }
    public BookingType BookingType { get; set; }
    public bool IsDeleted { get; set; }

    public Car Car { get; set; }
    public User User { get; set; }
    public RentalDetail? RentalDetail { get; set; }
    public ConsultationDetail? ConsultationDetail { get; set; }
}