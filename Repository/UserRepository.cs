using Microsoft.EntityFrameworkCore;
using Veloco.Data;
using Veloco.Enums;
using Veloco.Interfaces;
using Veloco.Models;
using Veloco.Repository;

namespace Veloce.Repository;

public class UserRepository(VeloceDbContext context) : GenericRepository<User>(context), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email && u.Status != UserStatus.Deleted);
    }
    
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Username == username && u.Status != UserStatus.Deleted);
    }

    public async Task<User?> GetWithProfileAsync(int id)
    {
        return await _dbSet
            .Include(x => x.ClientProfile)
            .Include(x => x.EmployeeProfile)
                .ThenInclude(e => e.Dealership)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber && u.Status != UserStatus.Deleted);
    }
    
    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _dbSet
            .Include(u => u.ClientProfile)
            .Include(u => u.EmployeeProfile)
            .ThenInclude(e => e.Dealership)
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }
}