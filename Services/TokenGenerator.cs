using System.Security.Cryptography;
using Veloco.Interfaces;

namespace Veloce.Services;

public class TokenGenerator : ITokenGenerator
{
    public string GenerateSecureToken()
    {
        var bytes = new byte[4];
        RandomNumberGenerator.Fill(bytes);
        var code = BitConverter.ToUInt32(bytes, 0) % 900000 + 100000;
        return code.ToString();
    }
}