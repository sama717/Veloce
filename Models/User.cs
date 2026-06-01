using Veloco.Enums;

namespace Veloco.Models;

public class User
{
    public int Id { get; set; }
    public string? ProfilePicture { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
}