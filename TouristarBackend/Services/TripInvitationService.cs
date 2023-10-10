using TouristarBackend.Contracts;
using TouristarModels.Models;

namespace TouristarBackend.Services;

public class TripInvitationService : ITripInvitationService
{
    private readonly IRepositoryManager _repository;
    private readonly ILogger<ITripInvitationService> _logger;

    public TripInvitationService(
        IRepositoryManager repository,
        ILogger<ITripInvitationService> logger
    )
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TripInvitation> CreateInvitation(long userId, long tripId, string email)
    {
        var existingInvitation = _repository.TripInvitation.GetInvitationByEmailAndTrip(
            email,
            tripId
        );
        if (existingInvitation != null)
            return null;
        var invitation = new TripInvitation
        {
            UserId = userId,
            TripId = tripId,
            InvitedEmail = email,
            Accepted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        _repository.TripInvitation.CreateInvitation(invitation);
        await _repository.Save();
        return _repository.TripInvitation.FindById(invitation.Id);
    }

    public TripInvitation? RetrieveInvitation(string email)
    {
        return _repository.TripInvitation.GetInvitationByEmail(email);
    }

    public TripInvitation UpdateInvitation(long id, UpdateTripInvitationDto data)
    {
        var invitation = _repository.TripInvitation.FindById(id);
        invitation.Accepted = data.Accepted ?? invitation.Accepted;
        _repository.Save();
        return invitation;
    }
}
