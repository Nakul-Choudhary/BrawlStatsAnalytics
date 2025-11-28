using BrawlStats.Domain.Entities;

namespace BrawlStats.Core.Interfaces
{
    public interface IPlayerBrawlerRepository
    {
        Task<PlayerBrawler?> GetByPlayerAndBrawlerAsync(string playerTag, int brawlerId);
        Task AddOrUpdateAsync(PlayerBrawler playerBrawler);
        Task AddOrUpdateRangeAsync(List<PlayerBrawler> playerBrawlers);

        // ✅ NEW: Create PlayerBrawlers from API data
        Task<List<PlayerBrawler>> CreateFromApiDataAsync(int playerId, string playerTag, List<(int apiBrawlerId, string name, int power, int rank, int trophies, int highestTrophies)> brawlerData);
    }
}