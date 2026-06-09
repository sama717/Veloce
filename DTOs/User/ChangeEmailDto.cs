using System.ComponentModel.DataAnnotations;

namespace Veloco.DTOs.User;

public class ChangeEmailDto
{
    [Required]
    [EmailAddress]
    public string NewEmail { get; set; }
    
    [Required]
    public string Password { get; set; }
}