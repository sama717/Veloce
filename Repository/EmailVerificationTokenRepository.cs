using Microsoft.EntityFrameworkCore;
using Veloco.Data;
using Veloco.Interfaces;
using Veloco.Models;
using Veloco.Repository;

namespace Veloce.Repository;

public class EmailVerificationTokenRepository(VeloceDbContext context) 
    : GenericRepository<EmailVerificationToken>(context), IEmailVerificationTokenRepository
{
    public async Task<EmailVerificationToken?> GetValidTokenAsync(string token)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.TokenHash == token
        &&  !t.IsUsed
        &&  t.ExpiresAt > DateTime.UtcNow
        );
    }
}