namespace Veloco.DTOs.Booking;

public class ConsultationDetailDto
{
    public int Id { get; set; }
    public DateTime PreferredDate { get; set; }
    public string? Notes { get; set; }
    public int DealershipId { get; set; }
}