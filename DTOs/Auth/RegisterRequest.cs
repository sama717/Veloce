using System.ComponentModel.DataAnnotations;
using Veloco.Enums;

namespace Veloco.DTOs.Auth;

public class RegisterRequest
{
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }
    
    [StringLength(50)]
    public string? MiddleName { get; set; }
    
    [Required]
    [StringLength(50)]
    public string LastName { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [Phone]
    public string PhoneNumber { get; set; }
    
    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string Username { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; }
    
    [Required]
    public UserMode Mode { get; set; }
}