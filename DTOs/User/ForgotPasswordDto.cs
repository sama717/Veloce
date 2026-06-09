using System.ComponentModel.DataAnnotations;

namespace Veloco.DTOs.User;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}