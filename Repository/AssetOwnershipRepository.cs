using Microsoft.EntityFrameworkCore;
using Veloco.Data;
using Veloco.Interfaces;
using Veloco.Models;
using Veloco.Repository;

namespace Veloce.Repository;

public class AssetOwnershipRepository(VeloceDbContext context)
    : GenericRepository<AssetOwnership>(context), IAssetOwnershipRepository
{
    public async Task<AssetOwnership?> GetByCarIdAsync(int carId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(ao => ao.CarId == carId);
    }

    public async Task<IEnumerable<AssetOwnership>> GetByDealershipIdAsync(int dealershipId)
    {
        return await _dbSet
            .Include(ao => ao.Car)
            .Where(ao => ao.DealershipId == dealershipId)
            .ToListAsync();
    }

    public async Task<IEnumerable<AssetOwnership>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(ao => ao.Car)
            .Where(ao => ao.UserId == userId)
            .ToListAsync();
    }
}