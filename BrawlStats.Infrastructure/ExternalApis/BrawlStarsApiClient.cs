// File: BrawlStats.Infrastructure/ExternalApis/BrawlStarsApiClient.cs
// REPLACE THE ENTIRE FILE WITH THIS

using BrawlStats.Core.DTOs;
using BrawlStats.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BrawlStats.Infrastructure.ExternalApis
{
    public class BrawlStarsApiClient : IBrawlStarsApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BrawlStarsApiClient> _logger;

        public BrawlStarsApiClient(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<BrawlStarsApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            _logger.LogInformation($"✨ API Client initialized");
            _logger.LogInformation($"🌐 BaseAddress: {_httpClient.BaseAddress}");
        }

        public async Task<PlayerDto?> GetPlayerAsync(string playerTag)
        {
            try
            {
                var encodedTag = Uri.EscapeDataString(playerTag);
                var url = $"players/{encodedTag}";

                _logger.LogInformation($"🔍 GET {url}");

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"❌ Status: {response.StatusCode} - {content}");
                    return null;
                }

                var player = JsonSerializer.Deserialize<PlayerDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _logger.LogInformation($"✅ Found player: {player?.Name} with {player?.Brawlers?.Count ?? 0} brawlers");
                return player;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"💥 Error getting player {playerTag}");
                return null;
            }
        }

        public async Task<List<BattleDto>> GetBattleLogAsync(string playerTag)
        {
            try
            {
                var encodedTag = Uri.EscapeDataString(playerTag);
                var url = $"players/{encodedTag}/battlelog";

                _logger.LogInformation($"🔍 GET {url}");

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"❌ Battle log failed: {response.StatusCode} - {errorContent}");
                    return new List<BattleDto>();
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<BattleLogResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _logger.LogInformation($"✅ Found {result?.Items?.Count ?? 0} battles");
                return result?.Items ?? new List<BattleDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"💥 Error getting battle log for {playerTag}");
                return new List<BattleDto>();
            }
        }

        public async Task<List<BrawlerDto>> GetBrawlersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("brawlers");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to get brawlers. Status: {response.StatusCode}");
                    return new List<BrawlerDto>();
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<BrawlerListResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result?.Items ?? new List<BrawlerDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting brawlers");
                return new List<BrawlerDto>();
            }
        }

        private class BattleLogResponse
        {
            public List<BattleDto> Items { get; set; } = new();
        }

        private class BrawlerListResponse
        {
            public List<BrawlerDto> Items { get; set; } = new();
        }
    }
}