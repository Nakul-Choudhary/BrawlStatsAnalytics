using BrawlStats.Domain.Enums;

namespace BrawlStats.Domain.Entities
{
    public class MetaSnapshot
    {
        public int Id { get; set; }
        public DateTime SnapshotDate { get; set; }

        // Brawler info
        public int BrawlerId { get; set; }
        public string BrawlerName { get; set; } = string.Empty;
        public Brawler Brawler { get; set; } = null!;

        // Meta stats
        public decimal PickRate { get; set; } // Percentage
        public decimal WinRate { get; set; } // Percentage
        public int TotalGames { get; set; }
        public int TotalWins { get; set; }

        // Analysis
        public TierRank Tier { get; set; }
        public string TrendDirection { get; set; } = "Stable"; // Rising, Falling, Stable
        public decimal TrendPercentage { get; set; } // % change from previous snapshot

        // Mode specific (optional)
        public string? Mode { get; set; }
        public string? Map { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
