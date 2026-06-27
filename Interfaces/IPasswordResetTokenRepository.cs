using Veloco.Models;

namespace Veloco.Interfaces;

public interface IPasswordResetTokenRepository : IRepository<PasswordResetToken>
{
    Task<PasswordResetToken?> GetValidTokenAsync(string tokenHash);
}