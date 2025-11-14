namespace BrawlStats.Domain.Entities
{
    public class PlayerBrawler
    {
        public int Id { get; set; }

        // Player info
        public int PlayerId { get; set; }
        public string PlayerTag { get; set; } = string.Empty;
        public Player Player { get; set; } = null!;

        // Brawler info
        public int BrawlerId { get; set; }
        public string BrawlerName { get; set; } = string.Empty;
        public Brawler Brawler { get; set; } = null!;

        // Stats
        public int Power { get; set; }
        public int Rank { get; set; }
        public int Trophies { get; set; }
        public int HighestTrophies { get; set; }

        // Custom calculated stats
        public int TotalBattles { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public decimal WinRate { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}