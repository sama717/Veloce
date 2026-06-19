using Veloco.Enums;

namespace Veloco.DTOs.User;

public class UpdateEmployeeDto
{
    public int? DealershipId { get; set; }
    public EmployeeMode? Position { get; set; }
}