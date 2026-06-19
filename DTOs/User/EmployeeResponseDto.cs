using Veloco.Enums;

namespace Veloco.DTOs.User;

public class EmployeeResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public int DealershipId { get; set; }
    public string DealershipName { get; set; }
    public EmployeeMode Position { get; set; }
}