using BrawlStats.Core.DTOs;

namespace BrawlStats.Core.Interfaces
{
    public interface IBrawlStarsApiClient
    {
        Task<PlayerDto?> GetPlayerAsync(string playerTag);
        Task<List<BattleDto>> GetBattleLogAsync(string playerTag);
        Task<List<BrawlerDto>> GetBrawlersAsync();
    }
}