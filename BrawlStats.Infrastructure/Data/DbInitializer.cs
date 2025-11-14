using BrawlStats.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BrawlStats.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(BrawlStatsDbContext context)
        {
            // Ensure database is created
            await context.Database.MigrateAsync();

            // Seed initial data if needed
            if (!await context.Brawlers.AnyAsync())
            {
                await SeedBrawlersAsync(context);
            }
        }

        private static async Task SeedBrawlersAsync(BrawlStatsDbContext context)
        {
            // Add some common brawlers (you'll get full list from API later)
            var brawlers = new List<Brawler>
            {
                new Brawler { BrawlerId = 16000000, Name = "Shelly" },
                new Brawler { BrawlerId = 16000001, Name = "Colt" },
                new Brawler { BrawlerId = 16000002, Name = "Bull" },
                new Brawler { BrawlerId = 16000003, Name = "Brock" },
                new Brawler { BrawlerId = 16000004, Name = "Rico" },
                new Brawler { BrawlerId = 16000005, Name = "Spike" },
                new Brawler { BrawlerId = 16000006, Name = "Barley" },
                new Brawler { BrawlerId = 16000007, Name = "Jessie" },
                new Brawler { BrawlerId = 16000008, Name = "Nita" },
                new Brawler { BrawlerId = 16000009, Name = "Dynamike" },
                new Brawler { BrawlerId = 16000010, Name = "El Primo" },
            };

            await context.Brawlers.AddRangeAsync(brawlers);
            await context.SaveChangesAsync();
        }
    }
}
