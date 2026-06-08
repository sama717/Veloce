namespace Veloco.Models;

public class Dealership
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public ICollection<Car> Cars { get; set; } = new List<Car>();
}