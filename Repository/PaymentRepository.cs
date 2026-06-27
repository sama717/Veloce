using Microsoft.EntityFrameworkCore;
using Veloco.Data;
using Veloco.Enums;
using Veloco.Interfaces;
using Veloco.Models;
using Veloco.Repository;

namespace Veloce.Repository;

public class PaymentRepository(VeloceDbContext context) : GenericRepository<Payment>(context), IPaymentRepository
{
    public async Task<IEnumerable<Payment>> GetByRentalDetailIdAsync(int rentalDetailId)
    {
        return await _dbSet
            .Where(p => p.RentalDetailId == rentalDetailId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status)
    {
        return await _dbSet
            .Where(p => p.Status == status)
            .ToListAsync();
    }

    public async Task<Payment?> GetByStripePaymentIdAsync(string stripePaymentId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.StripePaymentId == stripePaymentId);
    }
}