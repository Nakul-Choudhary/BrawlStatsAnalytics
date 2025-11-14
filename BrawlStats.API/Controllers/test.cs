using BrawlStats.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BrawlStats.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IBrawlStarsApiClient _apiClient;
        private readonly ILogger<TestController> _logger;

        public TestController(
            IBrawlStarsApiClient apiClient,
            ILogger<TestController> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        /// <summary>
        /// Test fetching player data from Brawl Stars API
        /// </summary>
        [HttpGet("player/{tag}")]
        public async Task<IActionResult> TestGetPlayer(string tag)
        {
            try
            {
                if (!tag.StartsWith("#"))
                    tag = "#" + tag;

                _logger.LogInformation($"Testing API call for player {tag}");

                var player = await _apiClient.GetPlayerAsync(tag);

                if (player == null)
                {
                    return NotFound(new { message = "Player not found in Brawl Stars API" });
                }

                return Ok(new
                {
                    success = true,
                    player = new
                    {
                        player.Tag,
                        player.Name,
                        player.Trophies,
                        brawlerCount = player.Brawlers?.Count ?? 0
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error testing player fetch for {tag}");
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Test fetching battle log from Brawl Stars API
        /// </summary>
        [HttpGet("battles/{tag}")]
        public async Task<IActionResult> TestGetBattles(string tag)
        {
            try
            {
                if (!tag.StartsWith("#"))
                    tag = "#" + tag;

                _logger.LogInformation($"Testing battle log fetch for {tag}");

                var battles = await _apiClient.GetBattleLogAsync(tag);

                if (battles == null || !battles.Any())
                {
                    return NotFound(new
                    {
                        message = "No battles found",
                        battleCount = 0
                    });
                }

                return Ok(new
                {
                    success = true,
                    battleCount = battles.Count,
                    firstBattle = battles.FirstOrDefault(),
                    allBattles = battles.Select(b => new
                    {
                        b.BattleTime,
                        b.Event.Mode,
                        b.Event.Map,
                        b.Battle.Result,
                        b.Battle.TrophyChange,
                        starPlayer = b.Battle.StarPlayer,
                        teamsCount = b.Battle.Teams?.Count ?? 0,
                        playersCount = b.Battle.Players?.Count ?? 0
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error testing battle fetch for {tag}");
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Test database connection
        /// </summary>
        [HttpGet("db-test")]
        public async Task<IActionResult> TestDatabase(
            [FromServices] BrawlStats.Infrastructure.Data.BrawlStatsDbContext context)
        {
            try
            {
                var playerCount = context.Players.Count();
                var battleCount = context.Battles.Count();

                return Ok(new
                {
                    success = true,
                    database = "Connected",
                    players = playerCount,
                    battles = battleCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }
    }
}