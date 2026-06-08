using Veloco.Enums;

namespace Veloco.Models;

public class ClientProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public UserMode Mode { get; set; } 
    public User User { get; set; } 
}