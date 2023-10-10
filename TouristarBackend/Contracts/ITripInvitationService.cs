using TouristarModels.Models;

namespace TouristarBackend.Contracts;

public interface ITripInvitationService
{
    Task<TripInvitation> CreateInvitation(long userId, long tripId, string email);
    TripInvitation? RetrieveInvitation(string email);
    TripInvitation UpdateInvitation(long id, UpdateTripInvitationDto data);
}
