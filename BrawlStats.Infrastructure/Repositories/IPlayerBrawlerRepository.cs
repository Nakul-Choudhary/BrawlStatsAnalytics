
// Generated code:
using System.Collections.Generic;
using System.Threading.Tasks;
using BrawlStats.Domain.Entities;

namespace BrawlStats.Core.Interfaces
{
    public interface IPlayerBrawlerRepository
    {
        Task<PlayerBrawler?> GetByPlayerAndBrawlerAsync(string playerTag, int brawlerId);
        Task AddOrUpdateAsync(PlayerBrawler playerBrawler);
        Task AddOrUpdateRangeAsync(List<PlayerBrawler> playerBrawlers);
    }
}