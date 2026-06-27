using System.ComponentModel.DataAnnotations;

namespace Veloco.DTOs.Booking;

public class CreateConsultationBookingDto
{
    [Required]
    public int CarId { get; set; }
    
    [Required]
    public int DealershipId { get; set; }
    
    [Required]
    public DateTime PreferredDate { get; set; }
    
    [StringLength(500)]
    public string? Notes { get; set; }
}