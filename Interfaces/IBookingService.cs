using Veloco.DTOs.Booking;
using Veloco.Models;

namespace Veloco.Interfaces;

public interface IBookingService
{
    Task<BookingDto> CreateRentalAsync(CreateRentalBookingDto dto, int userId);
    
    Task<BookingDto> CreateConsultationAsync(CreateConsultationBookingDto dto, int userId);
    
    Task<BookingDto> GetByIdAsync(int id);
    Task<IEnumerable<BookingDto>> GetByUserIdAsync(int userId);
    Task<IEnumerable<BookingDto>> GetByCarIdAsync(int carId);
    
    Task<BookingDto> UpdateStatusAsync(int id, UpdateBookingDto dto);
    
    Task CancelAsync(int id, User user);
    
    Task DeleteAsync(int id, User user);
}