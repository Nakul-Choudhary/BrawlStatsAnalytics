using BrawlStats.Core.Interfaces;
using BrawlStats.Domain.Entities;
using BrawlStats.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BrawlStats.Infrastructure.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly BrawlStatsDbContext _context;

        public PlayerRepository(BrawlStatsDbContext context)
        {
            _context = context;
        }

        public async Task<Player?> GetByTagAsync(string playerTag)
        {
            return await _context.Players
                .Include(p => p.PlayerBrawlers)
                .Include(p => p.TrophyHistory)
                .FirstOrDefaultAsync(p => p.PlayerTag == playerTag);
        }

        public async Task<Player?> GetByIdAsync(int id)
        {
            return await _context.Players
                .Include(p => p.PlayerBrawlers)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Player>> GetAllAsync()
        {
            return await _context.Players.ToListAsync();
        }

        public async Task AddAsync(Player player)
        {
            await _context.Players.AddAsync(player);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Player player)
        {
            _context.Players.Update(player);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string playerTag)
        {
            return await _context.Players.AnyAsync(p => p.PlayerTag == playerTag);
        }
    }
}