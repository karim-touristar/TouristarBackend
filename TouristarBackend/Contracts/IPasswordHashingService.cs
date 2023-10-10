using System.Security.Cryptography;

namespace TouristarBackend.Contracts;

public interface IPasswordHashingService
{
    bool VerifyPasswordHash(string password, byte[] passwordHash, HMACSHA512 hmac);
    (byte[] passwordHash, byte[] passwordSalt) CreatePasswordHash(string password, HMACSHA512 hmac);
}
