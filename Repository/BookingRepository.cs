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
            .Where(b => b.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByStatusAsync(BookingStatus status)
    {
        return await _dbSet
            .Where(b => b.Status == status)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByTypeAsync(BookingType bookingType)
    {
        return await _dbSet
            .Where(b => b.BookingType == bookingType)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByCarIdAsync(int carId)
    {
        return await _dbSet
            .Where(b => b.CarId == carId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        return await _dbSet
            .Include(b => b.RentalDetail)
            .Where(b => b.RentalDetail != null &&
                        b.RentalDetail.StartDate >= from &&
                        b.RentalDetail.EndDate <= to)
            .ToListAsync();
    }

    public async Task<Booking?> GetWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(b => b.RentalDetail)
            .Include(b => b.ConsultationDetail)
            .FirstOrDefaultAsync(b => b.Id == id);
    }
}