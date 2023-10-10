using TouristarModels.Models;

namespace TouristarBackend.Contracts;

public interface IAuthService
{
    Task<TokensResponseDto> Register(UserRequestDto request);
    TokensResponseDto Login(UserLoginDto request);
    Task<User> RegisterDeviceToken(long userId, string deviceToken);
    User GetUser(long userId);
    Task<User> UpdateUser(long userId, UpdateUserRequestDto data);
    Task<bool> DeleteAccount(long userId);
}
