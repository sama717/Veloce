namespace Veloco.Models;

public class Dealership
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    
    public ICollection<EmployeeProfile> Employees { get; set; } = new List<EmployeeProfile>();
    public ICollection<AssetOwnership> AssetOwnerships { get; set; } = new List<AssetOwnership>();
}