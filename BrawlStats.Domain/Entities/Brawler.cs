namespace BrawlStats.Domain.Entities
{
    public class Brawler
    {
        public int Id { get; set; }
        public int BrawlerId { get; set; } // From API
        public string Name { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }

        // Navigation properties
        public ICollection<PlayerBrawler> PlayerBrawlers { get; set; } = new List<PlayerBrawler>();
        public ICollection<MetaSnapshot> MetaSnapshots { get; set; } = new List<MetaSnapshot>();
    }
}
