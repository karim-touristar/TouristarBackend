using TouristarModels.Models;

namespace TouristarBackend.Contracts;

public interface IStatsService
{
    UserStatsDto GetUserStats(long userId);
}
