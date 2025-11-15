using BrawlStats.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BrawlStats.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly ILogger<PlayersController> _logger;

        public PlayersController(IPlayerService playerService, ILogger<PlayersController> logger)
        {
            _playerService = playerService;
            _logger = logger;
        }

        /// <summary>
        /// Get player analytics with custom metrics
        /// </summary>
        /// <param name="tag">Player tag (e.g., #2PP for testing or any valid tag)</param>
        [HttpGet("{tag}/analytics")]
        public async Task<IActionResult> GetPlayerAnalytics(string tag)
        {
            try
            {
                // Add # if not present
                if (!tag.StartsWith("#"))
                    tag = "#" + tag;

                var analytics = await _playerService.GetPlayerAnalyticsAsync(tag);

                if (analytics == null)
                    return NotFound(new { message = $"Player {tag} not found. Please track the player first using POST /api/players/track" });

                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting analytics for player {tag}");
                return StatusCode(500, new { message = "An error occurred while fetching player analytics" });
            }
        }

        /// <summary>
        /// Start tracking a player (fetches data from Brawl Stars API)
        /// </summary>
        /// <param name="request">Player tag to track</param>
        [HttpPost("track")]
        public async Task<IActionResult> TrackPlayer([FromBody] TrackPlayerRequest request)
        {
            try
            {
                var tag = request.PlayerTag;
                if (!tag.StartsWith("#"))
                    tag = "#" + tag;

                var success = await _playerService.TrackPlayerAsync(tag);

                if (!success)
                    return BadRequest(new { message = $"Could not track player {tag}. Please check the tag and try again." });

                return Ok(new { message = $"Successfully tracking player {tag}", playerTag = tag });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error tracking player {request.PlayerTag}");
                return StatusCode(500, new { message = "An error occurred while tracking the player" });
            }
        }

        /// <summary>
        /// Update player data manually
        /// </summary>
        [HttpPost("{tag}/update")]
        public async Task<IActionResult> UpdatePlayer(string tag)
        {
            try
            {
                if (!tag.StartsWith("#"))
                    tag = "#" + tag;

                await _playerService.UpdatePlayerDataAsync(tag);
                return Ok(new { message = $"Player {tag} data updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating player {tag}");
                return StatusCode(500, new { message = "An error occurred while updating player data" });
            }
        }
        // Add this to your PlayersController or wherever you have your API endpoints

        [HttpPost("manual")]
        public async Task<IActionResult> SaveManualStats([FromBody] ManualStatsRequest request)
        {
            try
            {
                // Validate the request
                if (string.IsNullOrEmpty(request.PlayerTag) || string.IsNullOrEmpty(request.Name))
                {
                    return BadRequest(new { message = "Player tag and name are required" });
                }

                // Here you would save to your database
                // For example:
                // await _dbContext.PlayerAnalytics.AddAsync(new PlayerAnalytics
                // {
                //     PlayerTag = request.PlayerTag,
                //     Name = request.Name,
                //     OverallStats = JsonSerializer.Serialize(request.OverallStats),
                //     CustomMetrics = JsonSerializer.Serialize(request.CustomMetrics),
                //     RecentForm = JsonSerializer.Serialize(request.RecentForm),
                //     CreatedAt = DateTime.UtcNow
                // });
                // await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    message = "Stats saved successfully",
                    playerTag = request.PlayerTag,
                    name = request.Name
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error saving stats: {ex.Message}" });
            }
        }

        // Add these models to your Models folder or at the top of your controller file
        public class ManualStatsRequest
        {
            public string PlayerTag { get; set; }
            public string Name { get; set; }
            public OverallStats OverallStats { get; set; }
            public CustomMetrics CustomMetrics { get; set; }
            public RecentForm RecentForm { get; set; }
            public List<object> BrawlerMastery { get; set; }
        }

        public class OverallStats
        {
            public int TotalTrophies { get; set; }
            public double WinRate { get; set; }
            public int TotalBattles { get; set; }
            public string FavoriteMode { get; set; }
        }

        public class CustomMetrics
        {
            public string SkillRating { get; set; }
            public double ConsistencyScore { get; set; }
            public double ClutchRating { get; set; }
            public string ImprovementTrend { get; set; }
        }

        public class RecentForm
        {
            public Last10Games Last10Games { get; set; }
        }

        public class Last10Games
        {
            public int Wins { get; set; }
            public int Losses { get; set; }
            public string TrendDirection { get; set; }
        }
    }

    public class TrackPlayerRequest
    {
        public string PlayerTag { get; set; } = string.Empty;
    }
}
