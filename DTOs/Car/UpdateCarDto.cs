using Veloco.Enums;

namespace Veloco.DTOs.Car;

public class UpdateCarDto
{
    public string? Color { get; set; }
    public int? Mileage { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public decimal? PricePerDay { get; set; }
    public int? Quantity { get; set; }
    public CarStatus? Status { get; set; }
}