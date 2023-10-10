using Microsoft.Extensions.Options;
using TouristarModels.Contracts;
using TouristarModels.Models;
using TouristarModels.Repositories;
using TouristarBackend.Contracts;
using TouristarBackend.Models;

namespace TouristarBackend.Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly DatabaseContext _dbContext;
    private IUserRepository? _userRepository;
    private ITripRepository? _tripRepository;
    private IActivityRepository? _activityRepository;
    private IDocumentRepository? _documentRepository;
    private IChecklistRepository? _checklistRepository;
    private IChecklistTaskRepository? _checklistTaskRepository;
    private ILocationRepository? _locationRepository;
    private IRadarRepository? _radarRepository;
    private IProcessedEmailRepository? _processedEmailRepository;
    private IFlightOperatorRepository? _flightOperatorRepository;
    private ITicketRepository? _ticketRepository;
    private IGooglePlacesRepository? _googlePlacesRepository;
    private IStorageRepository? _storageRepository;
    private IInvitationRepository? _invitationRepository;
    private ITripInvitationRepository? _tripInvitationRepository;

    private IOptionsMonitor<RadarConfig> _radarOptions;
    private IOptionsMonitor<GooglePlacesConfig> _googlePlacesOptions;

    private ILogger<IGooglePlacesRepository> _googleLogger;

    public RepositoryManager(
        DatabaseContext dbContext,
        IConfiguration configuration,
        IOptionsMonitor<RadarConfig> radarOptions,
        IOptionsMonitor<GooglePlacesConfig> googlePlacesOptions,
        ILogger<IGooglePlacesRepository> googleLogger
    )
    {
        _dbContext = dbContext;
        _radarOptions = radarOptions;
        _googlePlacesOptions = googlePlacesOptions;
        _googleLogger = googleLogger;
    }

    public IUserRepository User
    {
        get
        {
            _userRepository ??= new UserRepository(_dbContext);
            return _userRepository;
        }
    }

    public ITripRepository Trip
    {
        get
        {
            _tripRepository ??= new TripRepository(_dbContext);
            return _tripRepository;
        }
    }

    public IActivityRepository Activity
    {
        get
        {
            _activityRepository ??= new ActivityRepository(_dbContext);
            return _activityRepository;
        }
    }

    public IDocumentRepository Document
    {
        get
        {
            _documentRepository ??= new DocumentRepository(_dbContext);
            return _documentRepository;
        }
    }

    public IChecklistRepository Checklist
    {
        get
        {
            _checklistRepository ??= new ChecklistRepository(_dbContext);
            return _checklistRepository;
        }
    }

    public IChecklistTaskRepository ChecklistTask
    {
        get
        {
            _checklistTaskRepository ??= new ChecklistTaskRepository(_dbContext);
            return _checklistTaskRepository;
        }
    }

    public ILocationRepository Location
    {
        get
        {
            _locationRepository ??= new LocationRepository(_dbContext);
            return _locationRepository;
        }
    }

    public IRadarRepository Radar
    {
        get
        {
            _radarRepository ??= new RadarRepository(_radarOptions.CurrentValue);
            return _radarRepository;
        }
    }

    public IProcessedEmailRepository ProcessedEmail
    {
        get
        {
            _processedEmailRepository ??= new ProcessedEmailRepository(_dbContext);
            return _processedEmailRepository;
        }
    }

    public IFlightOperatorRepository FlightOperator
    {
        get
        {
            _flightOperatorRepository ??= new FlightOperatorRepository(_dbContext);
            return _flightOperatorRepository;
        }
    }

    public ITicketRepository Ticket
    {
        get
        {
            _ticketRepository ??= new TicketRepository(_dbContext);
            return _ticketRepository;
        }
    }

    public IGooglePlacesRepository GooglePlaces
    {
        get
        {
            _googlePlacesRepository ??= new GooglePlacesRepository(
                _googlePlacesOptions,
                _googleLogger
            );
            return _googlePlacesRepository;
        }
    }

    public IStorageRepository Storage
    {
        get
        {
            _storageRepository ??= new StorageRepository();
            return _storageRepository;
        }
    }

    public IInvitationRepository Invitation
    {
        get
        {
            _invitationRepository ??= new InvitationRepository(_dbContext);
            return _invitationRepository;
        }
    }

    public ITripInvitationRepository TripInvitation
    {
        get
        {
            _tripInvitationRepository ??= new TripInvitationRepository(_dbContext);
            return _tripInvitationRepository;
        }
    }

    async Task IRepositoryManager.Save()
    {
        await Task.Run(() => _dbContext.SaveChangesAsync());
    }
}
