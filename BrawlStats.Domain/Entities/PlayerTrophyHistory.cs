namespace BrawlStats.Domain.Entities
{
    public class PlayerTrophyHistory
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string PlayerTag { get; set; } = string.Empty;
        public Player Player { get; set; } = null!;

        public int Trophies { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
