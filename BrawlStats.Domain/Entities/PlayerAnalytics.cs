namespace BrawlStats.Domain.Entities
{
    public class PlayerAnalytics
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string PlayerTag { get; set; } = string.Empty;
        public Player Player { get; set; } = null!;

        // Custom metrics
        public int SkillRating { get; set; } // Custom ELO
        public decimal ConsistencyScore { get; set; } // 0-100
        public decimal ClutchRating { get; set; } // Performance under pressure
        public string ImprovementTrend { get; set; } = "Stable"; // Rising, Falling, Stable

        // Recent performance
        public int Last10Wins { get; set; }
        public int Last10Losses { get; set; }
        public decimal Last10WinRate { get; set; }

        // Favorite stats
        public string? FavoriteMode { get; set; }
        public string? FavoriteBrawler { get; set; }
        public string? BestTimeToPlay { get; set; } // Morning, Afternoon, Evening, Night

        public DateTime LastCalculated { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}