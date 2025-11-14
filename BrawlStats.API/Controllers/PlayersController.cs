using BrawlStats.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BrawlStats.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly IBattleRepository _battleRepository;
        private readonly ILogger<PlayersController> _logger;

        public PlayersController(
            IPlayerService playerService,
            IBattleRepository battleRepository,
            ILogger<PlayersController> logger)
        {
            _playerService = playerService;
            _battleRepository = battleRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get player analytics with custom metrics
        /// </summary>
        [HttpGet("{tag}/analytics")]
        public async Task<IActionResult> GetPlayerAnalytics(string tag)
        {
            try
            {
                // Add # if not present
                if (!tag.StartsWith("#"))
                    tag = "#" + tag;

                _logger.LogInformation($"Fetching analytics for {tag}");

                var analytics = await _playerService.GetPlayerAnalyticsAsync(tag);

                if (analytics == null)
                {
                    _logger.LogWarning($"Analytics returned null for {tag}");
                    return NotFound(new
                    {
                        message = $"Player {tag} not found. Please track the player first using POST /api/players/track"
                    });
                }

                // Debug: Check battle count
                var battleCount = await _battleRepository.GetByPlayerTagAsync(tag, 100);
                _logger.LogInformation($"Player {tag} has {battleCount.Count} battles in database");
                _logger.LogInformation($"Analytics: WinRate={analytics.OverallStats.WinRate}, Battles={analytics.OverallStats.TotalBattles}");

                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting analytics for player {tag}");
                return StatusCode(500, new
                {
                    message = "An error occurred while fetching player analytics",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Start tracking a player (fetches data from Brawl Stars API)
        /// </summary>
        [HttpPost("track")]
        public async Task<IActionResult> TrackPlayer([FromBody] TrackPlayerRequest request)
        {
            try
            {
                var tag = request.PlayerTag;
                if (!tag.StartsWith("#"))
                    tag = "#" + tag;

                _logger.LogInformation($"Tracking player {tag}");

                var success = await _playerService.TrackPlayerAsync(tag);

                if (!success)
                {
                    _logger.LogWarning($"Failed to track player {tag}");
                    return BadRequest(new
                    {
                        message = $"Could not track player {tag}. Please check the tag and try again."
                    });
                }

                // Check if battles were saved
                var battles = await _battleRepository.GetByPlayerTagAsync(tag, 10);
                _logger.LogInformation($"After tracking: Player {tag} has {battles.Count} battles");

                return Ok(new
                {
                    message = $"Successfully tracking player {tag}. Found {battles.Count} recent battles.",
                    playerTag = tag,
                    battlesFound = battles.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error tracking player {request.PlayerTag}");
                return StatusCode(500, new
                {
                    message = "An error occurred while tracking the player",
                    error = ex.Message
                });
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

                _logger.LogInformation($"Updating player {tag}");

                await _playerService.UpdatePlayerDataAsync(tag);

                var battles = await _battleRepository.GetByPlayerTagAsync(tag, 10);

                return Ok(new
                {
                    message = $"Player {tag} data updated successfully",
                    battlesCount = battles.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating player {tag}");
                return StatusCode(500, new
                {
                    message = "An error occurred while updating player data",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Debug endpoint to check battles
        /// </summary>
        [HttpGet("{tag}/debug")]
        public async Task<IActionResult> DebugPlayer(string tag)
        {
            try
            {
                if (!tag.StartsWith("#"))
                    tag = "#" + tag;

                var battles = await _battleRepository.GetByPlayerTagAsync(tag, 10);

                return Ok(new
                {
                    playerTag = tag,
                    totalBattles = battles.Count,
                    battles = battles.Select(b => new
                    {
                        b.BattleTime,
                        b.Mode,
                        b.Result,
                        b.TrophyChange,
                        b.IsStarPlayer,
                        b.BrawlerName
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in debug for {tag}");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class TrackPlayerRequest
    {
        public string PlayerTag { get; set; } = string.Empty;
    }
}