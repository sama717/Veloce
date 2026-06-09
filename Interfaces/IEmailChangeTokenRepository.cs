using Veloco.Models;

namespace Veloco.Interfaces;

public interface IEmailChangeTokenRepository : IRepository<EmailChangeToken>
{
    Task<EmailChangeToken?> GetValidTokenAsync(string tokenHash);
}