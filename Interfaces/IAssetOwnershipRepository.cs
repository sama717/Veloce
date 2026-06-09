using Veloco.Models;

namespace Veloco.Interfaces;

public interface IAssetOwnershipRepository : IRepository<AssetOwnership>
{
    Task<AssetOwnership?> GetByCarIdAsync(int carId);
    Task<IEnumerable<AssetOwnership>> GetByDealershipIdAsync(int dealershipId);
    Task<IEnumerable<AssetOwnership>> GetByUserIdAsync(int userId);
}