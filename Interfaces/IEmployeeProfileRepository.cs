using Veloco.Models;

namespace Veloco.Interfaces;

public interface IEmployeeProfileRepository : IRepository<EmployeeProfile>
{
    Task<IEnumerable<EmployeeProfile>> GetByDealershipIdAsync(int dealershipId);
    Task<EmployeeProfile?> GetByUserIdAsync(int userId);
}