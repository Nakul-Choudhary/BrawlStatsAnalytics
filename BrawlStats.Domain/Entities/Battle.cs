using BrawlStats.Domain.Enums;

namespace BrawlStats.Domain.Entities
{
    public class Battle
    {
        public int Id { get; set; }
        public string BattleTime { get; set; } = string.Empty; // API format: 20241114T123045.000Z
        public DateTime BattleDateTime { get; set; }
        public string Mode { get; set; } = string.Empty;
        public string? Map { get; set; }
        public BattleResult Result { get; set; }
        public int? TrophyChange { get; set; }
        public int? Duration { get; set; }
        public bool IsStarPlayer { get; set; }

        // Player info
        public string PlayerTag { get; set; } = string.Empty;
        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;

        // Brawler used
        public int BrawlerId { get; set; }
        public string BrawlerName { get; set; } = string.Empty;
        public int BrawlerPower { get; set; }
        public int BrawlerTrophies { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}