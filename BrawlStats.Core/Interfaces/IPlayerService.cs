using BrawlStats.Core.DTOs;

namespace BrawlStats.Core.Interfaces
{
    public interface IPlayerService
    {
        Task<PlayerAnalyticsDto?> GetPlayerAnalyticsAsync(string playerTag);
        Task<bool> TrackPlayerAsync(string playerTag);
        Task UpdatePlayerDataAsync(string playerTag);
    }
}