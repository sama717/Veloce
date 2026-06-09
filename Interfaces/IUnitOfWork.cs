using Veloco.Interfaces;

namespace Veloco.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ICarRepository Cars { get; }
    IBookingRepository Bookings { get; }
    IPaymentRepository Payments { get; }
    IDealershipRepository Dealerships { get; }
    IAssetOwnershipRepository AssetOwnerships { get; }
    IEmailChangeTokenRepository EmailChangeTokens { get; }
    IPhoneChangeTokenRepository PhoneChangeTokens { get; }
    IPasswordResetTokenRepository PasswordResetTokens { get; }
    
    Task<int> SaveChangesAsync();
}