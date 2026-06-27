namespace Veloco.Models;

public class CarImage
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }
    public int CarId { get; set; }
    public Car Car { get; set; }
    public bool IsMain { get; set; } = false;
    public int? DisplayOrder { get; set; } 
}