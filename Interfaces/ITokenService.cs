using Veloco.Models;

namespace Veloco.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}