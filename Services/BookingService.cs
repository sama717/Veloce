using AutoMapper;
using Veloce.Exceptions;
using Veloco.DTOs.Booking;
using Veloco.Enums;
using Veloco.Interfaces;
using Veloco.Models;

namespace Veloce.Services;

public class BookingService(IUnitOfWork unitOfWork, IMapper mapper) : IBookingService
{
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _unitOfWork =  unitOfWork;
    
    private async Task<bool> IsCarProvider(int userId, int carId)
    {
        var ownership = await _unitOfWork.AssetOwnerships.GetByCarIdAsync(carId);
        return ownership?.UserId == userId;
    }
    
    private async Task<int?> GetCarDealershipIdAsync(int carId)
    {
        var ownership = await _unitOfWork.AssetOwnerships.GetByCarIdAsync(carId);
        return ownership?.DealershipId;
    }
    
    public async Task<BookingDto> CreateRentalAsync(CreateRentalBookingDto dto, int userId)
    {
        var car = await _unitOfWork.Cars.GetByIdAsync(dto.CarId);
        if (car == null)
            throw new AppException("Car not found", 404);
        
        if (car.AvailableQuantity <= 0)
            throw new AppException("Car is not available", 400);
        
        var days = (dto.EndDate - dto.StartDate).Days;
        if (days <= 0)
            throw new AppException("End date must be after start date", 400);
        
        var totalPrice = days * (car.PricePerDay ?? 0);
        
        var booking = new Booking
        {
            UserId = userId,
            CarId = dto.CarId,
            BookingType = BookingType.Rental,
            Status = BookingStatus.Pending, 
            CreatedAt = DateTime.UtcNow
        };
        
        booking.RentalDetail = new RentalDetail
        {
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            VerificationDocument = dto.VerificationDocument,
            TotalPrice = totalPrice
        };
        
        car.AvailableQuantity--;
        _unitOfWork.Cars.Update(car);

        await _unitOfWork.Bookings.AddAsync(booking);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<BookingDto>(booking);
    }

    public async Task<BookingDto> CreateConsultationAsync(CreateConsultationBookingDto dto, int userId)
    {
        var car = await _unitOfWork.Cars.GetByIdAsync(dto.CarId);
        if (car == null)
            throw new AppException("Car not found", 404);
        
        var dealership = await _unitOfWork.Dealerships.GetByIdAsync(dto.DealershipId);
        if (dealership == null)
            throw new AppException("Dealership not found", 404);
        
        var booking = new Booking
        {
            UserId = userId,
            CarId = dto.CarId,
            BookingType = BookingType.Consultation,
            Status = BookingStatus.Confirmed, 
            CreatedAt = DateTime.UtcNow
        };
        
        booking.ConsultationDetail = new ConsultationDetail
        {
            DealershipId = dto.DealershipId,
            PreferredDate = dto.PreferredDate,
            Notes = dto.Notes
        };
        
        await _unitOfWork.Bookings.AddAsync(booking);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<BookingDto>(booking);
    }

    public async Task<BookingDto> GetByIdAsync(int id)
    {
        var booking = await _unitOfWork.Bookings.GetWithDetailsAsync(id);
        if (booking == null)
            throw new AppException("Booking not found", 404);

        return _mapper.Map<BookingDto>(booking);
    }

    public async Task<IEnumerable<BookingDto>> GetByUserIdAsync(int userId)
    {
        var bookings = await _unitOfWork.Bookings.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<BookingDto>>(bookings);
    }

    public async Task<IEnumerable<BookingDto>> GetByCarIdAsync(int carId)
    {
        var bookings = await _unitOfWork.Bookings.GetByCarIdAsync(carId);
        return _mapper.Map<IEnumerable<BookingDto>>(bookings);
    }

    public async Task<BookingDto> UpdateStatusAsync(int id, UpdateBookingDto dto)
    {
        var booking = await _unitOfWork.Bookings.GetWithDetailsAsync(id);
        if (booking == null)
            throw new AppException("Booking not found", 404);
        
        if (booking.Status == BookingStatus.Canceled || booking.Status == BookingStatus.Completed || booking.Status == BookingStatus.Rejected)
            throw new AppException($"Cannot update booking with status '{booking.Status}'", 400);

        booking.Status = dto.Status;
        
        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<BookingDto>(booking);
    }

    public async Task CancelAsync(int id, User user)
    {
        var booking = await _unitOfWork.Bookings.GetWithDetailsAsync(id);
        if (booking == null)
            throw new AppException("Booking not found", 404);
        
        var isBooker = booking.UserId == user.Id;
        var isProvider = await IsCarProvider(user.Id, booking.CarId);
        var isAdmin = user is { Role: UserRole.SystemUser, EmployeeProfile.Position: EmployeeMode.Admin };
        var isManager = user is { Role: UserRole.SystemUser, EmployeeProfile.Position: EmployeeMode.Manager };
           
        if (!isBooker && !isProvider && !isAdmin && !isManager)
            throw new AppException("You are not authorized to cancel this booking", 403);
        
        if (isManager)
        {
            var dealershipId = await GetCarDealershipIdAsync(booking.CarId);
            if (dealershipId != user.EmployeeProfile?.DealershipId)
                throw new AppException("You can only cancel bookings for cars in your dealership", 403);
        }
        
        if (!isAdmin && !isManager && booking.RentalDetail != null)
        {
            var daysUntilStart = (booking.RentalDetail.StartDate - DateTime.UtcNow).Days;
            if (daysUntilStart < 3)
                throw new AppException("Cannot cancel within 3 days of rental start", 400);
        }
        
        if (booking.Status == BookingStatus.Canceled || booking.Status == BookingStatus.Completed || booking.Status == BookingStatus.Rejected)
            throw new AppException($"Cannot cancel booking with status '{booking.Status}'", 400);
    
        booking.Status = BookingStatus.Canceled;
        
        if (booking.BookingType == BookingType.Rental && booking.RentalDetail != null)
        {
            var car = await _unitOfWork.Cars.GetByIdAsync(booking.CarId);
            if (car != null)
            {
                car.AvailableQuantity++;
                _unitOfWork.Cars.Update(car);
            }
        }
        
        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, User user)
    {
        var booking = await _unitOfWork.Bookings.GetWithDetailsAsync(id);
        if (booking == null)
            throw new AppException("Booking not found", 404);
        
        var isAdmin = user.Role == UserRole.SystemUser && user.EmployeeProfile?.Position == EmployeeMode.Admin;
        if (!isAdmin)
            throw new AppException("Only admins can delete bookings", 403);
        
        if (booking.Status == BookingStatus.Pending || booking.Status == BookingStatus.Confirmed)
            throw new AppException("Cannot delete an active booking. Cancel it first.", 400);
        
        booking.IsDeleted = true;
        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();
    }
}