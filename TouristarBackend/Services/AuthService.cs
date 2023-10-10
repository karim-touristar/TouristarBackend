using System.Security.Authentication;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using TouristarModels.Models;
using TouristarBackend.Contracts;
using TouristarBackend.Exceptions;
using TouristarBackend.Models;

namespace TouristarBackend.Services;

public class AuthService : IAuthService
{
    private readonly IRepositoryManager _repository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly ILogger _logger;
    private readonly string _jwtTokenKey;

    public AuthService(
        IRepositoryManager repository,
        IJwtService jwtService,
        IPasswordHashingService passwordHashingService,
        ILogger<AuthService> logger,
        IOptionsMonitor<AuthConfig> optionsMonitor
    )
    {
        _repository = repository;
        _jwtService = jwtService;
        _passwordHashingService = passwordHashingService;
        _logger = logger;
        _jwtTokenKey = optionsMonitor.CurrentValue.JwtTokenKey;
    }

    public async Task<TokensResponseDto> Register(UserRequestDto request)
    {
        var userCheck = _repository.User.FindOneByEmail(request.Email);
        if (userCheck != null)
        {
            throw new UserExistsException();
        }

        var (passwordHash, passwordSalt) = _passwordHashingService.CreatePasswordHash(
            request.Password,
            hmac: new HMACSHA512()
        );
        User user =
            new()
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                AppNotifications = true,
                FlightNotifications = true,
                ActivityNotifications = true
            };

        _repository.User.CreateUser(user);
        await _repository.Save();

        string? token = _jwtService.CreateToken(user.Id, _jwtTokenKey);

        if (token == null)
            throw new TokenGenerationException();

        TokensResponseDto tokensResponse = new() { AccessToken = token };
        return tokensResponse;
    }

    public TokensResponseDto Login(UserLoginDto request)
    {
        User? user = _repository.User.FindOneByEmail(request.Email);

        if (user == null)
        {
            throw new NotFoundException("A user with the provided credentials does not exist.");
        }

        if (
            !_passwordHashingService.VerifyPasswordHash(
                request.Password,
                user.PasswordHash,
                hmac: new HMACSHA512(user.PasswordSalt)
            )
        )
        {
            throw new AuthenticationException("Wrong password provided.");
        }

        string? token = _jwtService.CreateToken(user.Id, _jwtTokenKey);
        if (token == null)
            throw new TokenGenerationException();

        TokensResponseDto tokensResponse = new() { AccessToken = token };
        return tokensResponse;
    }

    public async Task<User> RegisterDeviceToken(long userId, string token)
    {
        var user = _repository.User.FindById(userId);
        user.DeviceToken = token;
        _repository.User.UpdateUser(user);
        await _repository.Save();
        return user;
    }

    public User GetUser(long userId)
    {
        var user = _repository.User.FindById(userId);
        // Obfuscate hash and salt.
        user.PasswordHash = null!;
        user.PasswordSalt = null!;
        return user;
    }

    public async Task<User> UpdateUser(long userId, UpdateUserRequestDto data)
    {
        var user = _repository.User.FindById(userId);
        user.Email = data.Email ?? user.Email;
        user.IsSyncingTickets = data.IsSyncingTickets ?? user.IsSyncingTickets;
        user.DeviceToken = data.DeviceToken ?? user.DeviceToken;
        user.AppNotifications = data.AppNotifications ?? user.AppNotifications;
        user.FlightNotifications = data.FlightNotifications ?? user.FlightNotifications;
        user.ActivityNotifications = data.ActivityNotifications ?? user.ActivityNotifications;
        await _repository.Save();
        return user;
    }

    public async Task<bool> DeleteAccount(long userId)
    {
        try
        {
            var user = _repository.User.FindById(userId);
            if (user == null)
            {
                throw new NotFoundException($"User with id {userId} not found.");
            }
            _repository.User.DeleteUser(user);
            await _repository.Save();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(
                $"There was an issue deleting account for user {userId}. Exception: {e}."
            );
            return false;
        }
    }
}
