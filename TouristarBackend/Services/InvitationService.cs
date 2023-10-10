using Microsoft.Extensions.Options;
using TouristarBackend.Constants;
using TouristarBackend.Contracts;
using TouristarBackend.Models;
using TouristarModels.Models;

namespace TouristarBackend.Services;

public class InvitationService : IInvitationService
{
    private readonly IJwtService _jwtService;
    private readonly IRepositoryManager _repository;
    private readonly ILogger<IInvitationService> _logger;
    private readonly string _jwtTokenKey;
    private readonly string _baseUrl;

    public InvitationService(
        IRepositoryManager repository,
        ILogger<IInvitationService> logger,
        IJwtService jwtService,
        IOptionsMonitor<AuthConfig> authOptions,
        IOptionsMonitor<InvitationConfig> invitationOptions
    )
    {
        _repository = repository;
        _logger = logger;
        _jwtService = jwtService;
        _jwtTokenKey = authOptions.CurrentValue.JwtTokenKey;
        _baseUrl = invitationOptions.CurrentValue.BaseUrl;
    }

    public async Task<InvitationUrlResponseDto> CreateOrRetrieveItineraryLink(
        long userId,
        long tripId
    )
    {
        var existingInvitation = _repository.Invitation.FindInvitationByTripId(tripId);
        if (existingInvitation != null)
            return new InvitationUrlResponseDto()
            {
                Url = BuildInvitationUrl(existingInvitation.Token)
            };

        var token = _jwtService.CreateToken(userId, _jwtTokenKey);
        var invitation = new Invitation
        {
            UserId = userId,
            TripId = tripId,
            Token = token
        };
        _repository.Invitation.CreateInvitation(invitation);
        await _repository.Save();
        return new InvitationUrlResponseDto() { Url = BuildInvitationUrl(invitation.Token) };
    }

    public PublicInvitationDto GetTripFromInvitation(string invitationToken)
    {
        var invitation = _repository.Invitation.FindInvitationByToken(invitationToken);
        var trip = _repository.Trip.FindTrip(invitation.TripId);
        var user = _repository.User.FindById(trip.UserId);
        return new() { Trip = trip, UserFullName = $"{user.Name}" };
    }

    private string BuildInvitationUrl(string token) => $"https://{_baseUrl}/?token={token}";
}
