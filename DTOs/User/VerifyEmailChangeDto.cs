using System.ComponentModel.DataAnnotations;

namespace Veloco.DTOs.User;

public class VerifyEmailChangeDto
{
    [Required]
    public string Token { get; set; }
}