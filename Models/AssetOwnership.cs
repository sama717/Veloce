namespace Veloco.Models;

public class AssetOwnership
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public int? UserId { get; set; }
    public int? DealershipId { get; set; }
    
    public User? User { get; set; }
    public Dealership? Dealership { get; set; }
    public Car Car { get; set; } 
}