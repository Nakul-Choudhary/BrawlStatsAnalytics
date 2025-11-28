namespace BrawlStats.Core.DTOs
{
    public class BattleDto
    {
        public string BattleTime { get; set; } = string.Empty;
        public EventDto Event { get; set; } = new();
        public BattleDetailsDto Battle { get; set; } = new();
    }

    public class EventDto
    {
        public int Id { get; set; }
        public string Mode { get; set; } = string.Empty;
        public string Map { get; set; } = string.Empty;
    }

    public class BattleDetailsDto
    {
        public string Mode { get; set; } = string.Empty;
        public string? Type { get; set; }
        public string? Result { get; set; }
        public int? Duration { get; set; }
        public int? TrophyChange { get; set; }
        public string? StarPlayer { get; set; }
        public List<List<BattlePlayerDto>> Teams { get; set; } = new();
        public List<BattlePlayerDto> Players { get; set; } = new();
    }

    public class BattlePlayerDto
    {
        // ✅ FIX: Tag is a string, not a nested object
        public string Tag { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public BattleBrawlerDto Brawler { get; set; } = new();
    }

    // ❌ REMOVE: BattlePlayerTagDto is not needed anymore

    public class BattleBrawlerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Power { get; set; }
        public int Trophies { get; set; }
    }
}