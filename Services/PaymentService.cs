using Microsoft.Extensions.Configuration;
using Stripe;
using Veloce.Exceptions;
using Veloco.DTOs.Payment;
using Veloco.Enums;
using Veloco.Interfaces;
using Veloco.Models;

namespace Veloce.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public PaymentService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
    }

    private async Task<(Booking booking, AssetOwnership ownership, decimal deposit)> ValidateAndGetBookingAsync(int bookingId)
    {
        var booking = await _unitOfWork.Bookings.GetWithDetailsAsync(bookingId);
        if (booking == null)
            throw new AppException("Booking not found", 404);

        if (booking.BookingType != BookingType.Rental)
            throw new AppException("Payment is not required for consultations", 400);

        if (booking.Status == BookingStatus.Confirmed)
            throw new AppException("Booking is already confirmed", 400);
        if (booking.Status == BookingStatus.Canceled || booking.Status == BookingStatus.Rejected)
            throw new AppException($"Cannot pay for a {booking.Status.ToString().ToLower()} booking", 400);

        var ownership = await _unitOfWork.AssetOwnerships.GetByCarIdAsync(booking.CarId);
        if (ownership == null)
            throw new AppException("Car ownership not found", 404);

        var deposit = booking.RentalDetail.TotalPrice * 0.2m;

        return (booking, ownership, deposit);
    }

    private (decimal dealershipCut, decimal ownerPayout) CalculateCuts(AssetOwnership ownership, decimal depositAmount)
    {
        if (ownership.DealershipId.HasValue)
        {
            return (depositAmount, 0);
        }
        else if (ownership.UserId.HasValue)
        {
            var dealershipCut = depositAmount * 0.2m;
            return (dealershipCut, depositAmount - dealershipCut);
        }

        return (0, 0);
    }

    public async Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(int bookingId)
    {
        var (booking, ownership, depositAmount) = await ValidateAndGetBookingAsync(bookingId);

        var (dealershipCut, ownerPayout) = CalculateCuts(ownership, depositAmount);

        var depositInCents = (long)(depositAmount * 100);

        var options = new PaymentIntentCreateOptions
        {
            Amount = depositInCents,
            Currency = "usd",
            Metadata = new Dictionary<string, string>
            {
                { "bookingId", booking.Id.ToString() },
                { "userId", booking.UserId.ToString() },
                { "carId", booking.CarId.ToString() }
            }
        };

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options);

        return new PaymentIntentResponseDto
        {
            ClientSecret = paymentIntent.ClientSecret,
            PaymentIntentId = paymentIntent.Id,
            Amount = depositAmount,
            DealershipCut = dealershipCut,
            OwnerPayout = ownerPayout
        };
    }

    public async Task<bool> ConfirmPaymentAsync(int bookingId, string paymentIntentId)
    {
        var (booking, ownership, depositAmount) = await ValidateAndGetBookingAsync(bookingId);
        
        var service = new PaymentIntentService();
        var paymentIntent = await service.GetAsync(paymentIntentId);

        if (paymentIntent.Status != "succeeded")
            throw new AppException("Payment not completed", 400);

        var (dealershipCut, ownerPayout) = CalculateCuts(ownership, depositAmount);
        
        var payment = new Payment
        {
            RentalDetailId = booking.RentalDetail.Id,
            Amount = depositAmount,
            Tax = 0,
            TotalAmount = depositAmount,
            DealershipCut = dealershipCut,
            OwnerPayout = ownerPayout,
            Status = PaymentStatus.Paid,
            StripePaymentId = paymentIntentId,
            CreatedAt = DateTime.UtcNow
        };
        
        booking.Status = BookingStatus.Confirmed;
        
        var car = await _unitOfWork.Cars.GetByIdAsync(booking.CarId);
        if (car != null)
        {
            car.AvailableQuantity--;
            _unitOfWork.Cars.Update(car);
        }

        await _unitOfWork.Payments.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}