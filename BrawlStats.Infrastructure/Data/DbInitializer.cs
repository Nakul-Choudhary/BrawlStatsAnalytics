using BrawlStats.Core.Interfaces;
using BrawlStats.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BrawlStats.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(BrawlStatsDbContext context, IServiceProvider serviceProvider)
        {
            // Ensure database is created
            await context.Database.MigrateAsync();

            // Seed brawlers from API
            if (!await context.Brawlers.AnyAsync())
            {
                Console.WriteLine("📥 No brawlers found, fetching from API...");
                await SeedBrawlersFromApiAsync(context, serviceProvider);
            }
            else
            {
                Console.WriteLine($"✅ Database already has {await context.Brawlers.CountAsync()} brawlers");
            }
        }

        private static async Task SeedBrawlersFromApiAsync(BrawlStatsDbContext context, IServiceProvider serviceProvider)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var apiClient = scope.ServiceProvider.GetRequiredService<IBrawlStarsApiClient>();

                Console.WriteLine("🔍 Fetching brawlers from Brawl Stars API...");
                var brawlersDto = await apiClient.GetBrawlersAsync();

                if (brawlersDto == null || !brawlersDto.Any())
                {
                    Console.WriteLine("⚠️ No brawlers returned from API, using fallback seed data");
                    await SeedFallbackBrawlers(context);
                    return;
                }

                var brawlers = brawlersDto.Select(b => new Brawler
                {
                    BrawlerId = b.Id,
                    Name = b.Name
                }).ToList();

                await context.Brawlers.AddRangeAsync(brawlers);
                await context.SaveChangesAsync();

                Console.WriteLine($"✅ Successfully seeded {brawlers.Count} brawlers from API");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error seeding brawlers from API: {ex.Message}");
                Console.WriteLine("⚠️ Using fallback seed data");
                await SeedFallbackBrawlers(context);
            }
        }

        private static async Task SeedFallbackBrawlers(BrawlStatsDbContext context)
        {
            // Fallback seed data with some common brawlers
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
            Console.WriteLine($"✅ Seeded {brawlers.Count} fallback brawlers");
        }
    }
}