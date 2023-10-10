using TouristarModels.Contracts;

namespace TouristarBackend.Contracts;

public interface IRepositoryManager
{
    IUserRepository User { get; }
    ITripRepository Trip { get; }
    IActivityRepository Activity { get; }
    IDocumentRepository Document { get; }
    IChecklistRepository Checklist { get; }
    IChecklistTaskRepository ChecklistTask { get; }
    ILocationRepository Location { get; }
    IRadarRepository Radar { get; }
    IProcessedEmailRepository ProcessedEmail { get; }
    IFlightOperatorRepository FlightOperator { get; }
    ITicketRepository Ticket { get; }
    IGooglePlacesRepository GooglePlaces { get; }
    IStorageRepository Storage { get; }
    IInvitationRepository Invitation { get; }
    ITripInvitationRepository TripInvitation { get; }
    Task Save();
}
