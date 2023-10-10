using TouristarBackend.Models;
using TouristarModels.Models;

namespace TouristarBackend.Constants;

public interface IInvitationService
{
    Task<InvitationUrlResponseDto> CreateOrRetrieveItineraryLink(long userId, long tripId);
    PublicInvitationDto GetTripFromInvitation(string invitationToken);
}
