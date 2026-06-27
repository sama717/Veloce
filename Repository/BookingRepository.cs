using Microsoft.EntityFrameworkCore;
using Veloco.Data;
using Veloco.Enums;
using Veloco.Interfaces;
using Veloco.Models;
using Veloco.Repository;

namespace Veloce.Repository;

public class BookingRepository(VeloceDbContext context) : GenericRepository<Booking>(context), IBookingRepository
{
    public async Task<IEnumerable<Booking>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(b => b.Car)
            .Include(b => b.RentalDetail)
            .Include(b => b.ConsultationDetail)
            .Where(b => b.UserId == userId && !b.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByStatusAsync(BookingStatus status)
    {
        return await _dbSet
            .Include(b => b.Car)
            .Include(b => b.User)
            .Where(b => b.Status == status && !b.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByTypeAsync(BookingType bookingType)
    {
        return await _dbSet
            .Include(b => b.Car)
            .Include(b => b.User)
            .Where(b => b.BookingType == bookingType && !b.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByCarIdAsync(int carId)
    {
        return await _dbSet
            .Include(b => b.User)
            .Include(b => b.RentalDetail)
            .Include(b => b.ConsultationDetail)
            .Where(b => b.CarId == carId && !b.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        return await _dbSet
            .Include(b => b.RentalDetail)
            .Include(b => b.Car)
            .Include(b => b.User)
            .Where(b => b.RentalDetail != null &&
                        b.RentalDetail.StartDate >= from &&
                        b.RentalDetail.EndDate <= to &&
                        !b.IsDeleted)
            .ToListAsync();
    }

    public async Task<Booking?> GetWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(b => b.User)
            .Include(b => b.Car)
            .Include(b => b.RentalDetail)
            .Include(b => b.ConsultationDetail)
                .ThenInclude(c => c.Dealership)
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
    }
    
    public async Task<IEnumerable<Booking>> GetProviderBookingsAsync(int userId)
    {
        return await _dbSet
            .Include(b => b.User)
            .Include(b => b.Car)
            .ThenInclude(c => c.AssetOwnership)
            .Include(b => b.RentalDetail)
            .Include(b => b.ConsultationDetail)
            .ThenInclude(c => c.Dealership)
            .Where(b => b.Car.AssetOwnership.UserId == userId)
            .Where(b => !b.IsDeleted)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }
}