using TouristarModels.Models;
using TouristarBackend.Contracts;
using TouristarBackend.Models;

namespace TouristarBackend.Services;

public class ChecklistTaskService : IChecklistTaskService
{
    private readonly IRepositoryManager _repository;
    private readonly ILogger _logger;

    public ChecklistTaskService(IRepositoryManager repository, ILogger<ChecklistTaskService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public IEnumerable<ChecklistTask> GetAllTasksForChecklist(long userId, long checklistId)
        => _repository.ChecklistTask.FindAllTasksForChecklist(userId, checklistId);

    public async Task<ChecklistTask> CreateChecklistTask(long userId, ChecklistTaskCreateDto task)
    {
        ChecklistTask taskToCreate = new()
        {
            Name = task.Name,
            IsChecked = task.IsChecked,
            ChecklistId = task.ChecklistId,
            UserId = userId,
        };
        _repository.ChecklistTask.CreateChecklistTask(taskToCreate);
        await _repository.Save();
        return taskToCreate;
    }

    public async Task<ChecklistTask> UpdateChecklistTask(long taskId, ChecklistTaskUpdateDto task)
    {
        var taskToUpdate = _repository.ChecklistTask.FindTask(taskId);
        taskToUpdate.Name = task.Name ?? taskToUpdate.Name;
        taskToUpdate.IsChecked = task.IsChecked ?? taskToUpdate.IsChecked;
        _repository.ChecklistTask.UpdateChecklistTask(taskToUpdate);
        await _repository.Save();
        return taskToUpdate;
    }

    public async Task<bool> DeleteChecklistTask(long taskId)
    {
        var task = _repository.ChecklistTask.FindTask(taskId);
        _repository.ChecklistTask.DeleteChecklistTask(task);
        await _repository.Save();
        return true;
    }
}