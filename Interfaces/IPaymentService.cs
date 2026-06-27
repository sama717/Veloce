using Veloco.DTOs.Payment;

namespace Veloco.Interfaces;

public interface IPaymentService
{
    Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(int bookingId);
    Task<bool> ConfirmPaymentAsync(int bookingId, string paymentIntentId);
}