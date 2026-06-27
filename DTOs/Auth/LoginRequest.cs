using System.ComponentModel.DataAnnotations;

namespace Veloco.DTOs.Auth;

public class LoginRequest
{
    [Required]
    public string Identifier { get; set; }
    
    [Required]
    public string Password { get; set; }
}