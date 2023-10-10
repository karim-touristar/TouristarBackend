using TouristarModels.Models;
using TouristarBackend.Contracts;
using TouristarBackend.Models;

namespace TouristarBackend.Services;

public class ChecklistService : IChecklistService
{
    private readonly IRepositoryManager _repository;
    private readonly ILogger _logger;

    public ChecklistService(IRepositoryManager repository, ILogger<ChecklistService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public IEnumerable<Checklist> GetAllChecklistsForTrip(long userId, long tripId)
        => _repository.Checklist.FindAllChecklistsForTrip(userId, tripId);

    public Checklist GetChecklist(long checklistId)
        => _repository.Checklist.FindChecklist(checklistId);

    public async Task<Checklist> CreateChecklist(long userId, ChecklistCreateDto checklist)
    {
        Checklist checklistToCreate = new()
        {
            Name = checklist.Name,
            UserId = checklist.UserId,
            TripId = checklist.TripId,
        };
        _repository.Checklist.CreateChecklist(checklistToCreate);
        await _repository.Save();
        return checklistToCreate;
    }

    public async Task<Checklist> UpdateChecklist(long checklistId, ChecklistUpdateDto checklist)
    {
        var checklistToUpdate = _repository.Checklist.FindChecklist(checklistId);
        checklistToUpdate.Name = checklist.Name ?? checklistToUpdate.Name;
        _repository.Checklist.UpdateChecklist(checklistToUpdate);
        await _repository.Save();
        return checklistToUpdate;
    }

    public async Task<bool> DeleteChecklist(long checklistId)
    {
        var checklist = _repository.Checklist.FindChecklist(checklistId);
        _repository.Checklist.DeleteChecklist(checklist);
        await _repository.Save();
        return true;
    }
}