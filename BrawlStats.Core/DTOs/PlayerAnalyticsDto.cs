namespace BrawlStats.Core.DTOs
{
    public class PlayerAnalyticsDto
    {
        public string PlayerTag { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public OverallStatsDto OverallStats { get; set; } = new();
        public CustomMetricsDto CustomMetrics { get; set; } = new();
        public List<BrawlerMasteryDto> BrawlerMastery { get; set; } = new();
        public RecentFormDto RecentForm { get; set; } = new();
    }

    public class OverallStatsDto
    {
        public int TotalTrophies { get; set; }
        public int HighestTrophies { get; set; }
        public decimal WinRate { get; set; }
        public int TotalBattles { get; set; }
        public string? FavoriteMode { get; set; }
    }

    public class CustomMetricsDto
    {
        public int SkillRating { get; set; }
        public decimal ConsistencyScore { get; set; }
        public string ImprovementTrend { get; set; } = "Stable";
        public decimal ClutchRating { get; set; }
    }

    public class BrawlerMasteryDto
    {
        public int BrawlerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MasteryLevel { get; set; } = string.Empty;
        public decimal WinRate { get; set; }
        public int GamesPlayed { get; set; }
        public int Trophies { get; set; }
    }

    public class RecentFormDto
    {
        public Last10GamesDto Last10Games { get; set; } = new();
    }

    public class Last10GamesDto
    {
        public int Wins { get; set; }
        public int Losses { get; set; }
        public string TrendDirection { get; set; } = "Stable";
    }
}
