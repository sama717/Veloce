using Veloco.Enums;

namespace Veloco.DTOs;

public class CarFilterParams
{
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? Color { get; set; }
    public CarCondition? Condition { get; set; }
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public ListingType? Type { get; set; }
}