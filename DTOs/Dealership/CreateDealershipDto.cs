using System.ComponentModel.DataAnnotations;

namespace Veloco.DTOs.Dealership;

public class CreateDealershipDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [Phone]
    public string PhoneNumber { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Address { get; set; }
    
    [Required]
    [StringLength(100)]
    public string City { get; set; }
    
    [Required]
    [StringLength(100)]
    public string State { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Country { get; set; }
}