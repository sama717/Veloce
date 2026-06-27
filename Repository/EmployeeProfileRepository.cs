using Microsoft.EntityFrameworkCore;
using Veloco.Data;
using Veloco.Interfaces;
using Veloco.Models;
using Veloco.Repository;

namespace Veloce.Repository;

public class EmployeeProfileRepository(VeloceDbContext context)
    : GenericRepository<EmployeeProfile>(context), IEmployeeProfileRepository
{
    public async Task<IEnumerable<EmployeeProfile>> GetByDealershipIdAsync(int dealershipId)
    {
        return await _dbSet
            .Include(ep => ep.User)
            .Include(ep => ep.Dealership)
            .Where(ep => ep.DealershipId == dealershipId)
            .ToListAsync();
    }

    public async Task<EmployeeProfile?> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(ep => ep.User)
            .Include(ep => ep.Dealership)
            .FirstOrDefaultAsync(ep => ep.UserId == userId);
    }
}