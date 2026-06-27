using Veloco.Models;

namespace Veloco.Interfaces;

public interface IPhoneChangeTokenRepository : IRepository<PhoneChangeToken>
{
    Task<PhoneChangeToken?> GetValidTokenAsync(string tokenHash);
}