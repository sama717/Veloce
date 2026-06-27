using Microsoft.EntityFrameworkCore;
using Veloco.Data;
using Veloco.Interfaces;
using Veloco.Models;
using Veloco.Repository;

namespace Veloce.Repository;

public class PasswordResetTokenRepository(VeloceDbContext context)
    : GenericRepository<PasswordResetToken>(context), IPasswordResetTokenRepository
{
    public async Task<PasswordResetToken?> GetValidTokenAsync(string tokenHash)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash 
                                      && !t.IsUsed 
                                      && t.ExpiresAt > DateTime.UtcNow);
    }
}