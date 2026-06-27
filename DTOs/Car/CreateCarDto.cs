using System.ComponentModel.DataAnnotations;
using Veloco.Enums;

namespace Veloco.DTOs.Car;

public class CreateCarDto
{
    [Required]
    [StringLength(50)]
    public string Brand { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Model { get; set; }
    
    [Required]
    [Range(1900, 2030)]
    public int Year { get; set; }
    
    [Required]
    [StringLength(30)]
    public string Color { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Mileage { get; set; }
    
    [Range(1, 20)]
    public int Seats { get; set; }
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal? Price { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal? PricePerDay { get; set; }
    
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    [Required]
    public ListingType Type { get; set; }
    
    [Required]
    public CarCondition Condition { get; set; }
    
    [Required]
    public int DealershipId { get; set; }
    
    public int? OwnerId { get; set; }
    public List<IFormFile>? Images { get; set; }
}