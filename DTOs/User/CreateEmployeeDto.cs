using System.ComponentModel.DataAnnotations;
using Veloco.Enums;

namespace Veloco.DTOs.User;

public class CreateEmployeeDto
{
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public int DealershipId { get; set; }
    
    [Required]
    public EmployeeMode Position { get; set; }
}