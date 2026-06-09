namespace Veloco.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ICarRepository Cars { get; }
    IBookingRepository Bookings { get; }
    IPaymentRepository Payments { get; }
    IDealershipRepository Dealerships { get; }
    IAssetOwnershipRepository AssetOwnerships { get; }
    
    Task<int> SaveChangesAsync();
}