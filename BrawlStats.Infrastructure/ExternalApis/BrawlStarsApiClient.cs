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
                    _logger.LogWarning($"Failed to get player {playerTag}. Status: {response.StatusCode}");
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

                Console.WriteLine($"⚔️ Fetching battles: {url}");
                Console.WriteLine($"📍 Full URL: {_httpClient.BaseAddress}{url}");
                _logger.LogInformation($"Fetching battle log: {url}");

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📊 Battle API Status: {response.StatusCode}");
                Console.WriteLine($"📄 Response length: {content.Length} characters");
                Console.WriteLine($"📄 First 200 chars: {content.Substring(0, Math.Min(200, content.Length))}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Battle log failed: {response.StatusCode}");
                    Console.WriteLine($"❌ Error response: {content}");
                    _logger.LogWarning($"Failed to get battle log. Status: {response.StatusCode}, Response: {content}");
                    return new List<BattleDto>();
                }

                var result = JsonSerializer.Deserialize<BattleLogResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var battles = result?.Items ?? new List<BattleDto>();
                Console.WriteLine($"✅ Found {battles.Count} battles");
                _logger.LogInformation($"Found {battles.Count} battles for {playerTag}");

                return battles;
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
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to get brawlers. Status: {response.StatusCode}");
                    return new List<BrawlerDto>();
                }

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
//}
//```

//## Step 2: Now track the player again

//1. * *Run your API**
//2. **Open the console window** (the black window that shows logs)
//3. **Call the track endpoint**:
//```
//POST http://localhost:5071/api/players/track
//{
//    "playerTag": "#8cGGYc2rg"
//}