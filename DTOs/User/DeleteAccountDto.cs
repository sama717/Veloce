using System.ComponentModel.DataAnnotations;

namespace Veloco.DTOs.User;

public class DeleteAccountDto
{
    [Required]
    public string Password { get; set; }
}