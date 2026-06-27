using Veloco.DTOs.User;

namespace Veloco.DTOs.Auth;

public class AuthResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string? ProfilePicture { get; set; }
    public string Role { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public bool IsEmailVerified { get; set; }
    public ClientProfileDto? ClientProfile { get; set; }
    public EmployeeProfileDto? EmployeeProfile { get; set; }
}