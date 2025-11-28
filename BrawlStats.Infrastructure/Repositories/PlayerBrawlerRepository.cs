using BrawlStats.Core.Interfaces;
using BrawlStats.Domain.Entities;
using BrawlStats.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BrawlStats.Infrastructure.Repositories
{
    public class PlayerBrawlerRepository : IPlayerBrawlerRepository
    {
        private readonly BrawlStatsDbContext _context;

        public PlayerBrawlerRepository(BrawlStatsDbContext context)
        {
            _context = context;
        }

        public async Task<PlayerBrawler?> GetByPlayerAndBrawlerAsync(string playerTag, int brawlerId)
        {
            return await _context.PlayerBrawlers
                .FirstOrDefaultAsync(pb => pb.PlayerTag == playerTag && pb.BrawlerId == brawlerId);
        }

        public async Task AddOrUpdateAsync(PlayerBrawler playerBrawler)
        {
            var existing = await GetByPlayerAndBrawlerAsync(playerBrawler.PlayerTag, playerBrawler.BrawlerId);

            if (existing != null)
            {
                existing.Power = playerBrawler.Power;
                existing.Rank = playerBrawler.Rank;
                existing.Trophies = playerBrawler.Trophies;
                existing.HighestTrophies = playerBrawler.HighestTrophies;
                existing.TotalBattles = playerBrawler.TotalBattles;
                existing.Wins = playerBrawler.Wins;
                existing.Losses = playerBrawler.Losses;
                existing.WinRate = playerBrawler.WinRate;
                existing.LastUpdated = DateTime.UtcNow;
                _context.PlayerBrawlers.Update(existing);
            }
            else
            {
                await _context.PlayerBrawlers.AddAsync(playerBrawler);
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddOrUpdateRangeAsync(List<PlayerBrawler> playerBrawlers)
        {
            foreach (var pb in playerBrawlers)
            {
                await AddOrUpdateAsync(pb);
            }
        }

        // ✅ NEW: Map API BrawlerIds to DB Ids
        public async Task<List<PlayerBrawler>> CreateFromApiDataAsync(
            int playerId,
            string playerTag,
            List<(int apiBrawlerId, string name, int power, int rank, int trophies, int highestTrophies)> brawlerData)
        {
            var playerBrawlers = new List<PlayerBrawler>();

            foreach (var data in brawlerData)
            {
                // Find the brawler in DB by API BrawlerId
                var dbBrawler = await _context.Brawlers
                    .FirstOrDefaultAsync(b => b.BrawlerId == data.apiBrawlerId);

                if (dbBrawler == null)
                {
                    Console.WriteLine($"⚠️ Brawler {data.name} (API ID: {data.apiBrawlerId}) not found in database, skipping");
                    continue;
                }

                playerBrawlers.Add(new PlayerBrawler
                {
                    PlayerId = playerId,
                    PlayerTag = playerTag,
                    BrawlerId = dbBrawler.Id, // ← Use DB Id, not API Id
                    BrawlerName = data.name,
                    Power = data.power,
                    Rank = data.rank,
                    Trophies = data.trophies,
                    HighestTrophies = data.highestTrophies,
                    TotalBattles = 0,
                    Wins = 0,
                    Losses = 0,
                    WinRate = 0,
                    LastUpdated = DateTime.UtcNow
                });
            }

            return playerBrawlers;
        }
    }
}