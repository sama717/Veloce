using Veloco.Data;
using Veloco.Interfaces;
using Veloce.Repository;

namespace Veloce.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly VeloceDbContext _context;

    public IUserRepository Users { get; }
    public ICarRepository Cars { get; }
    public IBookingRepository Bookings { get; }
    public IPaymentRepository Payments { get; }
    public IDealershipRepository Dealerships { get; }
    public IAssetOwnershipRepository AssetOwnerships { get; }
    public IEmailChangeTokenRepository EmailChangeTokens { get; }
    public IPhoneChangeTokenRepository PhoneChangeTokens { get; }
    public IPasswordResetTokenRepository PasswordResetTokens { get; }
    public IEmailVerificationTokenRepository EmailVerificationTokens { get; }
    public ICarImageRepository CarImages { get; }  

    public UnitOfWork(VeloceDbContext context)
    {
        _context = context;
        
        Users = new UserRepository(_context);
        Cars = new CarRepository(_context);
        Bookings = new BookingRepository(_context);
        Payments = new PaymentRepository(_context);
        Dealerships = new DealershipRepository(_context);
        AssetOwnerships = new AssetOwnershipRepository(_context);
        EmailChangeTokens = new EmailChangeTokenRepository(_context);
        PhoneChangeTokens = new PhoneChangeTokenRepository(_context);
        PasswordResetTokens = new PasswordResetTokenRepository(_context);
        EmailVerificationTokens = new EmailVerificationTokenRepository(_context);
        CarImages = new CarImageRepository(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}