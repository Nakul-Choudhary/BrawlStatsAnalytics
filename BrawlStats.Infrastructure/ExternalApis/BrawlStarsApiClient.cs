using BrawlStats.Core.DTOs;
using BrawlStats.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BrawlStats.Infrastructure.ExternalApis
{
    public class BrawlStarsApiClient : IBrawlStarsApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BrawlStarsApiClient> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl;


        public BrawlStarsApiClient(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<BrawlStarsApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _logger.LogWarning("🔐 Loaded API Key? " + (!string.IsNullOrEmpty(_apiKey)));
            _logger.LogWarning("🌐 Base URL: " + _httpClient.BaseAddress);

            _apiKey = configuration["BrawlStarsApi:ApiKey"] ?? throw new InvalidOperationException("API Key not found");
            _baseUrl = configuration["BrawlStarsApi:BaseUrl"] ?? "https://api.brawlstars.com/v1/";
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["BrawlStarsApi:ApiKey"];
            _baseUrl = configuration["BrawlStarsApi:BaseUrl"];
        }

        public async Task<PlayerDto?> GetPlayerAsync(string playerTag)
        {
            try
            {
                // Encode the tag (replace # with %23)
                var encodedTag = Uri.EscapeDataString(playerTag);
                var url = $"players/{encodedTag}";
                var response = await _httpClient.GetAsync(url);

                // 🔍 DEBUG LOGS
                var debugContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"DEBUG: Request URL = {url}");
                Console.WriteLine($"DEBUG: Status Code = {response.StatusCode}");
                Console.WriteLine($"DEBUG: Response = {debugContent}");

                if (!response.IsSuccessStatusCode)
                    return null;

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to get player {playerTag}. Status: {response.StatusCode}");
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var player = JsonSerializer.Deserialize<PlayerDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return player;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting player {playerTag}");
                return null;
            }
        }


        public async Task<List<BattleDto>> GetBattleLogAsync(string playerTag)
        {
            try
            {
                var encodedTag = Uri.EscapeDataString(playerTag);
                var response = await _httpClient.GetAsync($"/players/{encodedTag}/battlelog");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to get battle log for {playerTag}. Status: {response.StatusCode}");
                    return new List<BattleDto>();
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<BattleLogResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result?.Items ?? new List<BattleDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting battle log for {playerTag}");
                return new List<BattleDto>();
            }
        }

        public async Task<List<BrawlerDto>> GetBrawlersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/brawlers");

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