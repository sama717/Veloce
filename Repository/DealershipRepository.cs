using Microsoft.EntityFrameworkCore;
using Veloco.Data;
using Veloco.Interfaces;
using Veloco.Models;
using Veloco.Repository;

namespace Veloce.Repository;

public class DealershipRepository(VeloceDbContext context)
    : GenericRepository<Dealership>(context), IDealershipRepository
{
    public async Task<Dealership?> GetWithEmployeesAsync(int id)
    {
        return await _dbSet
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
    }

    public async Task<Dealership?> GetWithCarsAsync(int id)
    {
        return await _dbSet
            .Include(d => d.AssetOwnerships)
            .ThenInclude(ao => ao.Car)
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
    }

    public async Task<Dealership?> GetWithConsultationsAsync(int id)
    {
        return await _dbSet
            .Include(d => d.Consultations)
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
    }

    public async Task<IEnumerable<Dealership>> GetByCityAsync(string city)
    {
        return await _dbSet
            .Where(d => d.City == city && !d.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Dealership>> GetByStateAsync(string state)
    {
        return await _dbSet
            .Where(d => d.State == state && !d.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Dealership>> GetByCountryAsync(string country)
    {
        return await _dbSet
            .Where(d => d.Country == country && !d.IsDeleted)
            .ToListAsync();
    }
}