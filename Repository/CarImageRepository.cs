using Microsoft.EntityFrameworkCore;
using Veloco.Data;
using Veloco.Interfaces;
using Veloco.Models;
using Veloco.Repository;

namespace Veloce.Repository;

public class CarImageRepository(VeloceDbContext context)
    : GenericRepository<CarImage>(context), ICarImageRepository
{
    public async Task<IEnumerable<CarImage>> GetByCarIdAsync(int carId)
    {
        return await _dbSet
            .Where(img => img.CarId == carId)
            .ToListAsync();
    }
}