namespace BrawlStats.Core.DTOs
{
    public class PlayerDto
    {
        public string Tag { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Trophies { get; set; }
        public int HighestTrophies { get; set; }
        public int ExpLevel { get; set; }
        public int ExpPoints { get; set; }
        public int IsQualifiedFromChampionshipChallenge { get; set; }
        public int ThreeVsThreeVictories { get; set; }
        public int SoloVictories { get; set; }
        public int DuoVictories { get; set; }
        public int BestRoboRumbleTime { get; set; }
        public int BestTimeAsBigBrawler { get; set; }
        public ClubDto? Club { get; set; }
        public List<PlayerBrawlerDto> Brawlers { get; set; } = new();
    }

    public class ClubDto
    {
        public string Tag { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class PlayerBrawlerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Power { get; set; }
        public int Rank { get; set; }
        public int Trophies { get; set; }
        public int HighestTrophies { get; set; }
        public List<StarPowerDto> StarPowers { get; set; } = new();
        public List<GadgetDto> Gadgets { get; set; } = new();
    }

    public class StarPowerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class GadgetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
