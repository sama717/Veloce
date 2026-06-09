using Microsoft.EntityFrameworkCore;
using Veloco.Data;
using Veloco.Interfaces;
using Veloco.Models;
using Veloco.Repository;

namespace Veloce.Repository;

public class UserRepository(VeloceDbContext context) : GenericRepository<User>(context), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    { 
        return await _dbSet.FirstOrDefaultAsync(x => x.Email == email);
    }
    
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Username == username);
    }

    public async Task<User?> GetWithProfileAsync(int id)
    {
        return await _dbSet
            .Include(x => x.ClientProfile)
            .Include(x => x.EmployeeProfile)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}