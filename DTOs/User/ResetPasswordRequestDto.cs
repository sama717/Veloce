using System.ComponentModel.DataAnnotations;

namespace Veloco.DTOs.User;

public class ResetPasswordRequestDto
{
    [Required]
    
    public string Token { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string NewPassword { get; set; }
    
    [Required]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }
}