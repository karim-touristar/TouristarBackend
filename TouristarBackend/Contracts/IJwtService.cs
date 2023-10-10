using TouristarModels.Models;

namespace TouristarBackend.Contracts;

public interface IJwtService
{
    string CreateToken(long userId, string? tokenKey);
}

