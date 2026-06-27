using Veloco.Enums;

namespace Veloco.Models;

public class EmployeeProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int DealershipId { get; set; }
    public EmployeeMode Position { get; set; }
    
    public User User { get; set; } 
    public Dealership Dealership { get; set; }  
}