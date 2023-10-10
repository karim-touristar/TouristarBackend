using TouristarModels.Models;

namespace TouristarBackend.Contracts;

public interface IChecklistTaskService
{
    IEnumerable<ChecklistTask> GetAllTasksForChecklist(long userId, long checklistId);
    Task<ChecklistTask> CreateChecklistTask(long userId, ChecklistTaskCreateDto task);
    Task<ChecklistTask> UpdateChecklistTask(long taskId, ChecklistTaskUpdateDto task);
    Task<bool> DeleteChecklistTask(long taskId);
}