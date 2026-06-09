using System.ComponentModel.DataAnnotations;

namespace Veloco.DTOs.User;

public class VerifyPhoneNumberChangeDto
{
    [Required]
    public string Token { get; set; }
}