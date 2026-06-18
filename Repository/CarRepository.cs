using Microsoft.EntityFrameworkCore;
using Veloco.Data;
using Veloco.DTOs;
using Veloco.Enums;
using Veloco.Interfaces;
using Veloco.Models;
using Veloco.Repository;

namespace Veloce.Repository;

public class CarRepository(VeloceDbContext context) : GenericRepository<Car>(context), ICarRepository
{
    public async Task<IEnumerable<Car>> GetFilteredAsync(CarFilterParams carFilterParams)
    {
        return await _dbSet
            .Where(c => c.Status != CarStatus.Deleted)
            .Where(c => 
                (carFilterParams.Brand == null || c.Brand == carFilterParams.Brand) &&
                (carFilterParams.Model == null || c.Model == carFilterParams.Model) &&
                (carFilterParams.Color == null || c.Color == carFilterParams.Color) &&
                (carFilterParams.Condition == null || c.Condition == carFilterParams.Condition) &&
                (carFilterParams.YearFrom == null || c.Year >= carFilterParams.YearFrom) &&
                (carFilterParams.YearTo == null || c.Year <= carFilterParams.YearTo) &&
                (carFilterParams.MinPrice == null || c.Price >= carFilterParams.MinPrice) &&
                (carFilterParams.MaxPrice == null || c.Price <= carFilterParams.MaxPrice) &&
                (carFilterParams.Type == null || c.Type == carFilterParams.Type)
            )
            .ToListAsync();
    }

    public async Task<IEnumerable<Car>> GetByDealershipAsync(int dealershipId)
    {
        return await _dbSet
            .Include(i => i.AssetOwnership)
            .Where(c => c.Status != CarStatus.Deleted)
            .Where(i => i.AssetOwnership.DealershipId == dealershipId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Car>> GetAvailableForRentAsync()
    {
        return await _dbSet
            .Where(c => c.Status != CarStatus.Deleted)
            .Where(i => i.Type == ListingType.Rent && i.AvailableQuantity > 0)
            .ToListAsync();
    }

    public async Task<IEnumerable<Car>> GetAvailableForSaleAsync()
    {
        return await _dbSet
            .Where(c => c.Status != CarStatus.Deleted)
            .Where(i => i.Type == ListingType.Sale && i.AvailableQuantity > 0)
            .ToListAsync();
    }

    public async Task<Car?> GetWithImagesAsync(int id)
    {
        return await _dbSet
            .Include(i => i.Images)
            .Include(i => i.AssetOwnership)
                .ThenInclude(a => a.User)
            .FirstOrDefaultAsync(i => i.Id == id);
    }
}