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
        }

        public async Task<PlayerDto?> GetPlayerAsync(string playerTag)
        {
            try
            {
                var encodedTag = Uri.EscapeDataString(playerTag);
                var url = $"players/{encodedTag}";

                Console.WriteLine($"🔍 Fetching player: {url}");
                _logger.LogInformation($"Fetching player: {url}");

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Failed: {response.StatusCode}");
                    Console.WriteLine($"❌ Response: {content}");
                    _logger.LogWarning($"Failed to get player {playerTag}. Status: {response.StatusCode}, Response: {content}");
                    return null;
                }

                Console.WriteLine($"✅ Player data received");

                var player = JsonSerializer.Deserialize<PlayerDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return player;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Error: {ex.Message}");
                _logger.LogError(ex, $"Error getting player {playerTag}");
                return null;
            }
        }

        public async Task<List<BattleDto>> GetBattleLogAsync(string playerTag)
        {
            try
            {
                var encodedTag = Uri.EscapeDataString(playerTag);
                var url = $"players/{encodedTag}/battlelog";

                Console.WriteLine($"⚔️ Fetching battles for: {playerTag}");
                Console.WriteLine($"📍 Full URL: {_httpClient.BaseAddress}{url}");
                _logger.LogInformation($"Fetching battle log: {url}");

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📊 Battle API Status: {response.StatusCode}");
                Console.WriteLine($"📄 Response length: {content.Length} characters");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Battle log failed: {response.StatusCode}");
                    Console.WriteLine($"❌ Error response: {content}");
                    _logger.LogWarning($"Failed to get battle log for {playerTag}. Status: {response.StatusCode}, Response: {content}");

                    // Check for specific error codes
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Console.WriteLine($"⚠️ Player {playerTag} not found or has no battle log");
                        _logger.LogWarning($"Player {playerTag} not found or battle log unavailable");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        Console.WriteLine($"⚠️ Rate limit exceeded! Wait before making more requests");
                        _logger.LogWarning($"Rate limit exceeded for battle log request");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        Console.WriteLine($"⚠️ API key might be invalid or expired");
                        _logger.LogError($"API authentication failed - check API key");
                    }

                    return new List<BattleDto>();
                }

                // Log a preview of the response
                var preview = content.Length > 500 ? content.Substring(0, 500) : content;
                Console.WriteLine($"📄 Response preview: {preview}");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<BattleLogResponse>(content, options);

                var battles = result?.Items ?? new List<BattleDto>();
                Console.WriteLine($"✅ Found {battles.Count} battles for {playerTag}");

                if (battles.Count == 0)
                {
                    Console.WriteLine($"⚠️ Player {playerTag} has no battles in their battle log");
                    Console.WriteLine($"⚠️ This could mean: player is new, inactive, or has privacy settings enabled");
                    _logger.LogWarning($"Player {playerTag} has empty battle log");
                }

                _logger.LogInformation($"Found {battles.Count} battles for {playerTag}");

                return battles;
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"💥 JSON parsing error: {jsonEx.Message}");
                _logger.LogError(jsonEx, $"JSON error parsing battle log for {playerTag}");
                return new List<BattleDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Battle log error: {ex.Message}");
                Console.WriteLine($"💥 Stack trace: {ex.StackTrace}");
                _logger.LogError(ex, $"Error getting battle log for {playerTag}");
                return new List<BattleDto>();
            }
        }

        public async Task<List<BrawlerDto>> GetBrawlersAsync()
        {
            try
            {
                var url = "brawlers";

                Console.WriteLine($"🎮 Fetching brawlers list");

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Failed to get brawlers: {response.StatusCode}");
                    _logger.LogWarning($"Failed to get brawlers. Status: {response.StatusCode}");
                    return new List<BrawlerDto>();
                }

                Console.WriteLine($"✅ Brawlers data received");

                var result = JsonSerializer.Deserialize<BrawlerListResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var brawlers = result?.Items ?? new List<BrawlerDto>();
                Console.WriteLine($"✅ Found {brawlers.Count} brawlers");

                return brawlers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Error fetching brawlers: {ex.Message}");
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