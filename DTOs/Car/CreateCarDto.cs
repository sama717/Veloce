using Veloco.Enums;

namespace Veloco.DTOs.Car;

public class CreateCarDto
{
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string Color { get; set; }
    public int Mileage { get; set; }
    public int Seats { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public decimal? PricePerDay { get; set; }
    public int Quantity { get; set; }
    public ListingType Type { get; set; }
    public CarCondition Condition { get; set; }
    public int DealershipId { get; set; }
}