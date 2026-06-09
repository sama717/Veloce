using Veloco.Enums;
using Veloco.Models;

namespace Veloco.Interfaces;

public interface IBookingRepository : IRepository<Booking>
{
    Task<IEnumerable<Booking>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Booking>> GetByStatusAsync(BookingStatus status);
    Task<IEnumerable<Booking>> GetByTypeAsync(BookingType bookingType);
    Task<IEnumerable<Booking>> GetByCarIdAsync(int carId);
    Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<Booking?> GetWithDetailsAsync(int id);
}