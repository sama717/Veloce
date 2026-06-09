using Microsoft.EntityFrameworkCore;
using Veloco.Data;
using Veloco.Interfaces;
using Veloco.Models;
using Veloco.Repository;

namespace Veloce.Repository;

public class EmailChangeTokenRepository(VeloceDbContext context)
    : GenericRepository<EmailChangeToken>(context), IEmailChangeTokenRepository
{
    public async Task<EmailChangeToken?> GetValidTokenAsync(string tokenHash)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash 
                                      && !t.IsUsed 
                                      && t.ExpiresAt > DateTime.UtcNow);
    }
}