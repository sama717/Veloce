using System.ComponentModel.DataAnnotations;

namespace Veloco.DTOs.User;

public class DeactivateAccountDto
{
    [Required]
    public string Password { get; set; }
}