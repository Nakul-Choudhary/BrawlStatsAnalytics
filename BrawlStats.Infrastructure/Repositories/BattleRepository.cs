using BrawlStats.Core.Interfaces;
using BrawlStats.Domain.Entities;
using BrawlStats.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BrawlStats.Infrastructure.Repositories
{
    public class BattleRepository : IBattleRepository
    {
        private readonly BrawlStatsDbContext _context;

        public BattleRepository(BrawlStatsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Battle>> GetByPlayerTagAsync(string playerTag, int count = 25)
        {
            return await _context.Battles
                .Where(b => b.PlayerTag == playerTag)
                .OrderByDescending(b => b.BattleDateTime)
                .Take(count)
                .ToListAsync();
        }

        public async Task AddAsync(Battle battle)
        {
            await _context.Battles.AddAsync(battle);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<Battle> battles)
        {
            await _context.Battles.AddRangeAsync(battles);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string playerTag, string battleTime)
        {
            return await _context.Battles
                .AnyAsync(b => b.PlayerTag == playerTag && b.BattleTime == battleTime);
        }

        public async Task<List<Battle>> GetRecentBattlesAsync(DateTime since)
        {
            return await _context.Battles
                .Where(b => b.BattleDateTime >= since)
                .ToListAsync();
        }
    }
}