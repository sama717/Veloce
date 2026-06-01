namespace Veloco.Models;

public class CarImage
{
    public string ImageUrl { get; set; }
    public int CarId { get; set; }
    
    public Car Car { get; set; }
}