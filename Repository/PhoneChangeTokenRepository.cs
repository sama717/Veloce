using Microsoft.EntityFrameworkCore;
using Veloco.Data;
using Veloco.Interfaces;
using Veloco.Models;
using Veloco.Repository;

namespace Veloce.Repository;

public class PhoneChangeTokenRepository(VeloceDbContext context)
    : GenericRepository<PhoneChangeToken>(context), IPhoneChangeTokenRepository
{
    public async Task<PhoneChangeToken?> GetValidTokenAsync(string tokenHash)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash 
                                      && !t.IsUsed 
                                      && t.ExpiresAt > DateTime.UtcNow);
    }
}