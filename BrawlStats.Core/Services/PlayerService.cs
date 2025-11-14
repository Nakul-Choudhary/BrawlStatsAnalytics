using BrawlStats.Core.DTOs;
using BrawlStats.Core.Interfaces;
using BrawlStats.Domain.Entities;
using BrawlStats.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace BrawlStats.Core.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IBattleRepository _battleRepository;
        private readonly IBrawlStarsApiClient _apiClient;
        private readonly ILogger<PlayerService> _logger;

        public PlayerService(
            IPlayerRepository playerRepository,
            IBattleRepository battleRepository,
            IBrawlStarsApiClient apiClient,
            ILogger<PlayerService> logger)
        {
            _playerRepository = playerRepository;
            _battleRepository = battleRepository;
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<PlayerAnalyticsDto?> GetPlayerAnalyticsAsync(string playerTag)
        {
            var player = await _playerRepository.GetByTagAsync(playerTag);
            if (player == null)
            {
                _logger.LogWarning($"Player {playerTag} not found");
                return null;
            }

            var battles = await _battleRepository.GetByPlayerTagAsync(playerTag, 100);

            var analytics = new PlayerAnalyticsDto
            {
                PlayerTag = player.PlayerTag,
                Name = player.Name,
                OverallStats = CalculateOverallStats(player, battles),
                CustomMetrics = CalculateCustomMetrics(player, battles),
                BrawlerMastery = CalculateBrawlerMastery(player.PlayerBrawlers.ToList()),
                RecentForm = CalculateRecentForm(battles)
            };

            return analytics;
        }

        public async Task<bool> TrackPlayerAsync(string playerTag)
        {
            try
            {
                // Check if already tracked
                if (await _playerRepository.ExistsAsync(playerTag))
                {
                    _logger.LogInformation($"Player {playerTag} already tracked");
                    return true;
                }

                // Fetch from API
                var playerDto = await _apiClient.GetPlayerAsync(playerTag);
                if (playerDto == null)
                {
                    _logger.LogWarning($"Could not fetch player {playerTag} from API");
                    return false;
                }

                // Create player entity
                var player = new Player
                {
                    PlayerTag = playerDto.Tag,
                    Name = playerDto.Name,
                    Trophies = playerDto.Trophies,
                    HighestTrophies = playerDto.HighestTrophies,
                    ExpLevel = playerDto.ExpLevel,
                    ExpPoints = playerDto.ExpPoints,
                    Victories3v3 = playerDto.ThreeVsThreeVictories,
                    SoloVictories = playerDto.SoloVictories,
                    DuoVictories = playerDto.DuoVictories,
                    ClubTag = playerDto.Club?.Tag,
                    ClubName = playerDto.Club?.Name,
                    LastUpdated = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                await _playerRepository.AddAsync(player);

                // Fetch and store initial battles
                await UpdatePlayerDataAsync(playerTag);

                _logger.LogInformation($"Successfully tracked player {playerTag}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error tracking player {playerTag}");
                return false;
            }
        }

        public async Task UpdatePlayerDataAsync(string playerTag)
        {
            try
            {
                var playerDto = await _apiClient.GetPlayerAsync(playerTag);
                if (playerDto == null) return;

                var player = await _playerRepository.GetByTagAsync(playerTag);
                if (player == null) return;

                // Update player stats
                player.Trophies = playerDto.Trophies;
                player.HighestTrophies = playerDto.HighestTrophies;
                player.ExpLevel = playerDto.ExpLevel;
                player.Victories3v3 = playerDto.ThreeVsThreeVictories;
                player.SoloVictories = playerDto.SoloVictories;
                player.DuoVictories = playerDto.DuoVictories;
                player.LastUpdated = DateTime.UtcNow;

                await _playerRepository.UpdateAsync(player);

                // Fetch and store new battles
                var battles = await _apiClient.GetBattleLogAsync(playerTag);
                var newBattles = new List<Battle>();

                foreach (var battleDto in battles)
                {
                    if (await _battleRepository.ExistsAsync(playerTag, battleDto.BattleTime))
                        continue;

                    var battle = MapBattleDtoToEntity(battleDto, player, playerTag);
                    newBattles.Add(battle);
                }

                if (newBattles.Any())
                {
                    await _battleRepository.AddRangeAsync(newBattles);
                    _logger.LogInformation($"Added {newBattles.Count} new battles for {playerTag}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating player data for {playerTag}");
            }
        }

        private Battle MapBattleDtoToEntity(BattleDto dto, Player player, string playerTag)
        {
            var playerInBattle = FindPlayerInBattle(dto, playerTag);

            return new Battle
            {
                BattleTime = dto.BattleTime,
                BattleDateTime = ParseBattleTime(dto.BattleTime),
                Mode = dto.Event.Mode,
                Map = dto.Event.Map,
                Result = ParseResult(dto.Battle.Result),
                TrophyChange = dto.Battle.TrophyChange,
                Duration = dto.Battle.Duration,
                IsStarPlayer = dto.Battle.StarPlayer == playerTag,  // ← FIXED: Direct string comparison
                PlayerTag = playerTag,
                PlayerId = player.Id,
                BrawlerId = playerInBattle?.Brawler.Id ?? 0,
                BrawlerName = playerInBattle?.Brawler.Name ?? "Unknown",
                BrawlerPower = playerInBattle?.Brawler.Power ?? 0,
                BrawlerTrophies = playerInBattle?.Brawler.Trophies ?? 0,
                CreatedAt = DateTime.UtcNow
            };
        }

        private BattlePlayerDto? FindPlayerInBattle(BattleDto dto, string playerTag)
        {
            foreach (var team in dto.Battle.Teams)
            {
                var player = team.FirstOrDefault(p => p.Tag.Tag == playerTag);
                if (player != null) return player;
            }

            return dto.Battle.Players.FirstOrDefault(p => p.Tag.Tag == playerTag);
        }

        private DateTime ParseBattleTime(string battleTime)
        {
            // Format: 20241114T123045.000Z
            return DateTime.TryParse(battleTime, out var result) ? result : DateTime.UtcNow;
        }

        private BattleResult ParseResult(string? result)
        {
            return result?.ToLower() switch
            {
                "victory" => BattleResult.Victory,
                "defeat" => BattleResult.Defeat,
                _ => BattleResult.Draw
            };
        }

        private OverallStatsDto CalculateOverallStats(Player player, List<Battle> battles)
        {
            var wins = battles.Count(b => b.Result == BattleResult.Victory);
            var total = battles.Count;

            return new OverallStatsDto
            {
                TotalTrophies = player.Trophies,
                HighestTrophies = player.HighestTrophies,
                WinRate = total > 0 ? (decimal)wins / total * 100 : 0,
                TotalBattles = total,
                FavoriteMode = battles.GroupBy(b => b.Mode)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault()?.Key
            };
        }

        private CustomMetricsDto CalculateCustomMetrics(Player player, List<Battle> battles)
        {
            var recent10 = battles.Take(10).ToList();
            var wins = recent10.Count(b => b.Result == BattleResult.Victory);

            return new CustomMetricsDto
            {
                SkillRating = CalculateSkillRating(player, battles),
                ConsistencyScore = CalculateConsistency(battles),
                ImprovementTrend = wins >= 6 ? "Rising" : wins <= 3 ? "Falling" : "Stable",
                ClutchRating = CalculateClutchRating(battles)
            };
        }

        private int CalculateSkillRating(Player player, List<Battle> battles)
        {
            int baseRating = 1500;
            var wins = battles.Count(b => b.Result == BattleResult.Victory);
            var total = battles.Count;

            if (total == 0) return baseRating;

            var winRate = (decimal)wins / total;
            var adjustment = (int)((winRate - 0.5m) * 1000);

            return Math.Clamp(baseRating + adjustment, 1000, 2500);
        }

        private decimal CalculateConsistency(List<Battle> battles)
        {
            if (battles.Count < 5) return 50m;

            var trophyChanges = battles
                .Where(b => b.TrophyChange.HasValue)
                .Select(b => (decimal)b.TrophyChange!.Value)
                .ToList();

            if (!trophyChanges.Any()) return 50m;

            var stdDev = CalculateStandardDeviation(trophyChanges);
            var consistency = Math.Max(0, 100 - stdDev * 5);

            return Math.Clamp(consistency, 0, 100);
        }

        private decimal CalculateClutchRating(List<Battle> battles)
        {
            var starPlayerCount = battles.Count(b => b.IsStarPlayer);
            var total = battles.Count;

            if (total == 0) return 50m;

            return (decimal)starPlayerCount / total * 100;
        }

        private decimal CalculateStandardDeviation(List<decimal> values)
        {
            if (values.Count == 0) return 0;

            var avg = values.Average();
            var sumOfSquares = values.Sum(v => (v - avg) * (v - avg));
            return (decimal)Math.Sqrt((double)(sumOfSquares / values.Count));
        }

        private List<BrawlerMasteryDto> CalculateBrawlerMastery(List<PlayerBrawler> brawlers)
        {
            return brawlers
                .OrderByDescending(b => b.Trophies)
                .Take(5)
                .Select(b => new BrawlerMasteryDto
                {
                    BrawlerId = b.BrawlerId,
                    Name = b.BrawlerName,
                    MasteryLevel = GetMasteryLevel(b.Rank),
                    WinRate = b.WinRate,
                    GamesPlayed = b.TotalBattles,
                    Trophies = b.Trophies
                })
                .ToList();
        }

        private string GetMasteryLevel(int rank)
        {
            return rank switch
            {
                >= 30 => "Master",
                >= 25 => "Expert",
                >= 20 => "Advanced",
                >= 15 => "Intermediate",
                _ => "Beginner"
            };
        }

        private RecentFormDto CalculateRecentForm(List<Battle> battles)
        {
            var last10 = battles.Take(10).ToList();
            var wins = last10.Count(b => b.Result == BattleResult.Victory);
            var losses = last10.Count(b => b.Result == BattleResult.Defeat);

            return new RecentFormDto
            {
                Last10Games = new Last10GamesDto
                {
                    Wins = wins,
                    Losses = losses,
                    TrendDirection = wins >= 6 ? "Up" : wins <= 3 ? "Down" : "Stable"
                }
            };
        }
    }
}