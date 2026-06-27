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
        var query = _dbSet
            .Include(c => c.Images)
            .Where(c => c.Status != CarStatus.Deleted);

        // String filters with partial match (case-insensitive)
        if (!string.IsNullOrWhiteSpace(carFilterParams.Brand))
            query = query.Where(c => EF.Functions.ILike(c.Brand, $"%{carFilterParams.Brand}%"));

        if (!string.IsNullOrWhiteSpace(carFilterParams.Model))
            query = query.Where(c => EF.Functions.ILike(c.Model, $"%{carFilterParams.Model}%"));

        if (!string.IsNullOrWhiteSpace(carFilterParams.Color))
            query = query.Where(c => EF.Functions.ILike(c.Color, $"%{carFilterParams.Color}%"));

        // Numeric/Exact filters
        if (carFilterParams.Condition.HasValue)
            query = query.Where(c => c.Condition == carFilterParams.Condition.Value);

        if (carFilterParams.YearFrom.HasValue)
            query = query.Where(c => c.Year >= carFilterParams.YearFrom.Value);

        if (carFilterParams.YearTo.HasValue)
            query = query.Where(c => c.Year <= carFilterParams.YearTo.Value);

        if (carFilterParams.MinPrice.HasValue)
            query = query.Where(c => c.Price >= carFilterParams.MinPrice.Value);

        if (carFilterParams.MaxPrice.HasValue)
            query = query.Where(c => c.Price <= carFilterParams.MaxPrice.Value);

        if (carFilterParams.Type.HasValue)
            query = query.Where(c => c.Type == carFilterParams.Type.Value);

        return await query.ToListAsync();
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
    
    public async Task<IEnumerable<Car>> GetMyCarsAsync(int userId)
    {
        return await _dbSet
            .Include(c => c.Images)
            .Include(c => c.AssetOwnership)
            .Where(c => c.AssetOwnership.UserId == userId)
            .Where(c => c.Status != CarStatus.Deleted)
            .ToListAsync();
    }
}