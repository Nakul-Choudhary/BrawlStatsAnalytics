namespace BrawlStats.Core.DTOs
{
    public class BrawlerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<StarPowerDto> StarPowers { get; set; } = new();
        public List<GadgetDto> Gadgets { get; set; } = new();
    }
}
