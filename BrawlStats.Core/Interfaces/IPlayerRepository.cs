using BrawlStats.Domain.Entities;

namespace BrawlStats.Core.Interfaces
{
    public interface IPlayerRepository
    {
        Task<Player?> GetByTagAsync(string playerTag);
        Task<Player?> GetByIdAsync(int id);
        Task<List<Player>> GetAllAsync();
        Task AddAsync(Player player);
        Task UpdateAsync(Player player);
        Task<bool> ExistsAsync(string playerTag);
    }
}