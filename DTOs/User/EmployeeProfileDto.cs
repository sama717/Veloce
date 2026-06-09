using Veloco.Enums;

namespace Veloco.DTOs.User;

public class EmployeeProfileDto
{
    public int Id { get; set; }
    public EmployeeMode Position { get; set; }
    public int DealershipId { get; set; }
    public string DealershipName { get; set; }
}