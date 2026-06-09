using Veloco.Data;
using Veloco.Interfaces;

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

    public UnitOfWork(
        VeloceDbContext context,
        IUserRepository users, 
        ICarRepository cars, 
        IBookingRepository bookings, 
        IPaymentRepository payments, 
        IDealershipRepository dealerships, 
        IAssetOwnershipRepository assetOwnerships)
    {
        _context = context;
        Users = users;
        Cars = cars;
        Bookings = bookings;
        Payments = payments;
        Dealerships = dealerships;
        AssetOwnerships = assetOwnerships;
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