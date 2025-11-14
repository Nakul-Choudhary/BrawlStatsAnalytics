using BrawlStats.Domain.Entities;

namespace BrawlStats.Core.Interfaces
{
    public interface IBattleRepository
    {
        Task<List<Battle>> GetByPlayerTagAsync(string playerTag, int count = 25);
        Task AddAsync(Battle battle);
        Task AddRangeAsync(List<Battle> battles);
        Task<bool> ExistsAsync(string playerTag, string battleTime);
        Task<List<Battle>> GetRecentBattlesAsync(DateTime since);
    }
}