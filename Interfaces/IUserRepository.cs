using Veloco.Models;

namespace Veloco.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetWithProfileAsync(int id);
    Task<User?> GetByPhoneNumberAsync(string phoneNumber);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
}