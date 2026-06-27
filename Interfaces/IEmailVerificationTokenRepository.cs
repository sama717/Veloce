using Veloco.Models;

namespace Veloco.Interfaces;

public interface IEmailVerificationTokenRepository : IRepository<EmailVerificationToken>
{
    Task<EmailVerificationToken?> GetValidTokenAsync(string token); 
}