using TouristarModels.Models;

namespace TouristarBackend.Contracts;

public interface IChecklistService
{
    IEnumerable<Checklist> GetAllChecklistsForTrip(long userId, long tripId);
    Checklist GetChecklist(long checklistId);
    Task<Checklist> CreateChecklist(long userId, ChecklistCreateDto checklist);
    Task<Checklist> UpdateChecklist(long checklistId, ChecklistUpdateDto checklist);
    Task<bool> DeleteChecklist(long checklistId);
}