using System.Security.Cryptography;
using TouristarBackend.Contracts;

namespace TouristarBackend.Services;

public class PasswordHashingService : IPasswordHashingService
{
    public (byte[] passwordHash, byte[] passwordSalt) CreatePasswordHash(
        string password,
        HMACSHA512 hmac
    )
    {
        return (
            passwordHash: hmac.ComputeHash(buffer: System.Text.Encoding.UTF8.GetBytes(password)),
            passwordSalt: hmac.Key
        );
    }

    public bool VerifyPasswordHash(string password, byte[] passwordHash, HMACSHA512 hmac)
    {
        var computeHash = hmac.ComputeHash(buffer: System.Text.Encoding.UTF8.GetBytes(password));
        return computeHash.SequenceEqual(passwordHash);
    }
}