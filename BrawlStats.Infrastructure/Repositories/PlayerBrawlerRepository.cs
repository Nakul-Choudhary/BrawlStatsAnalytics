// BrawlStats.Infrastructure/Repositories/PlayerBrawlerRepository.cs
using BrawlStats.Core.Interfaces;
using BrawlStats.Domain.Entities;
using BrawlStats.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
}