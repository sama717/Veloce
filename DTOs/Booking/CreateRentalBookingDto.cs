using System.ComponentModel.DataAnnotations;

namespace Veloco.DTOs.Booking;

public class CreateRentalBookingDto
{
    [Required]
    public int CarId { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [Required]
    public string VerificationDocument { get; set; }
}