using Veloce.Exceptions;
using Veloco.DTOs.Rental;
using Veloco.Enums;
using Veloco.Interfaces;
using Veloco.Templates;

namespace Veloce.Services;

public class RentalContractService(IUnitOfWork unitOfWork) : IRentalContractService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    
    public async Task<RentalContractDto> GetContractDataAsync(int bookingId)
    {
        var booking = await _unitOfWork.Bookings.GetWithDetailsAsync(bookingId);
        if (booking == null)
            throw new AppException("Booking not found", 404);

        if (booking.BookingType != BookingType.Rental)
            throw new AppException("Contract only available for rentals", 400);

        if (booking.Status != BookingStatus.Confirmed && booking.Status != BookingStatus.Completed)
            throw new AppException("Contract only available for confirmed or completed bookings", 400);

        var user = await _unitOfWork.Users.GetByIdAsync(booking.UserId);
        var car = await _unitOfWork.Cars.GetByIdAsync(booking.CarId);
        var rentalDetail = booking.RentalDetail;

        var payments = await _unitOfWork.Payments.GetByRentalDetailIdAsync(rentalDetail.Id);
        var payment = payments.FirstOrDefault();

        var totalDays = (rentalDetail.EndDate - rentalDetail.StartDate).Days;
        
        var assetOwnership = await _unitOfWork.AssetOwnerships.GetByCarIdAsync(booking.CarId);
        string dealershipName = "Veloce";

        if (assetOwnership?.DealershipId.HasValue == true)
        {
            var dealership = await _unitOfWork.Dealerships.GetByIdAsync(assetOwnership.DealershipId.Value);
            dealershipName = dealership?.Name ?? "Veloce";
        }
        else if (assetOwnership?.UserId.HasValue == true)
        {
            dealershipName = "Veloce (via Provider)";
        }

        return new RentalContractDto
        {
            BookingId = booking.Id,
            CustomerName = $"{user.FirstName} {user.LastName}",
            CustomerEmail = user.Email,
            CustomerPhone = user.PhoneNumber,
            CarBrand = car.Brand,
            CarModel = car.Model,
            CarYear = car.Year,
            StartDate = rentalDetail.StartDate,
            EndDate = rentalDetail.EndDate,
            TotalDays = totalDays,
            TotalPrice = rentalDetail.TotalPrice,
            DepositPaid = payment?.Amount ?? 0,
            GeneratedAt = DateTime.UtcNow,
            DealershipName = dealershipName
        };
    }

    public async Task<byte[]> GenerateContractAsync(int bookingId)
    {
        var data = await GetContractDataAsync(bookingId);
        return RentalContractTemplate.Generate(data);
    }
}