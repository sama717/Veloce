using Veloco.Enums;
using Veloco.Models;

namespace Veloco.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<IEnumerable<Payment>> GetByRentalDetailIdAsync(int rentalDetailId);
    Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status);
    Task<Payment?> GetByStripePaymentIdAsync(string stripePaymentId);
}