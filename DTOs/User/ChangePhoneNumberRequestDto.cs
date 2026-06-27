using System.ComponentModel.DataAnnotations;

namespace Veloco.DTOs.User;

public class ChangePhoneNumberRequestDto
{
    [Required]
    [Phone]
    public string NewPhoneNumber { get; set; }
    
    [Required]
    public string Password { get; set; }
}