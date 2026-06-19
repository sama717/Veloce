namespace Veloco.DTOs.Rental;

public class RentalContractDto
{
    public int BookingId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerPhone { get; set; }
    public string CarBrand { get; set; }
    public string CarModel { get; set; }
    public int CarYear { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal DepositPaid { get; set; }
    public DateTime GeneratedAt { get; set; }
    public string DealershipName { get; set; }
}