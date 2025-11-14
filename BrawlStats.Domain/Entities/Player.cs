namespace BrawlStats.Domain.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public string PlayerTag { get; set; } = string.Empty; // #ABC123
        public string Name { get; set; } = string.Empty;
        public int Trophies { get; set; }
        public int HighestTrophies { get; set; }
        public int ExpLevel { get; set; }
        public int ExpPoints { get; set; }
        public int Victories3v3 { get; set; }
        public int SoloVictories { get; set; }
        public int DuoVictories { get; set; }
        public string? ClubTag { get; set; }
        public string? ClubName { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<Battle> Battles { get; set; } = new List<Battle>();
        public ICollection<PlayerBrawler> PlayerBrawlers { get; set; } = new List<PlayerBrawler>();
        public ICollection<PlayerTrophyHistory> TrophyHistory { get; set; } = new List<PlayerTrophyHistory>();
    }
}