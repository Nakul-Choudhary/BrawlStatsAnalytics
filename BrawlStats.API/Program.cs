using BrawlStats.Core.Interfaces;
using BrawlStats.Core.Services;
using BrawlStats.Infrastructure.Data;
using BrawlStats.Infrastructure.ExternalApis;
using BrawlStats.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Brawl Stats Analytics API",
        Version = "v1",
        Description = "Advanced analytics platform for Brawl Stars with custom metrics and predictions"
    });
});

// Database
builder.Services.AddDbContext<BrawlStatsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// HttpClient for Brawl Stars API
builder.Services.AddHttpClient<IBrawlStarsApiClient, BrawlStarsApiClient>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var baseUrl = config["BrawlStarsApi:BaseUrl"];
    var apiKey = config["BrawlStarsApi:ApiKey"];

    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    client.DefaultRequestHeaders.Add("Accept", "application/json");

    Console.WriteLine("Final HttpClient BaseAddress = " + client.BaseAddress);
    Console.WriteLine("API KEY LOADED? " + (apiKey != null));
});

// Repositories
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IBattleRepository, BattleRepository>();
builder.Services.AddScoped<IPlayerBrawlerRepository, PlayerBrawlerRepository>();

// Services
builder.Services.AddScoped<IPlayerService, PlayerService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Initialize database with service provider for API access
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BrawlStatsDbContext>();
    await DbInitializer.InitializeAsync(context, scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Welcome endpoint
app.MapGet("/", () => new
{
    message = "Welcome to Brawl Stats Analytics API",
    version = "1.0.0",
    endpoints = new[]
    {
        "POST /api/players/track - Start tracking a player",
        "GET /api/players/{tag}/analytics - Get player analytics",
        "POST /api/players/{tag}/update - Update player data"
    },
    documentation = "/swagger"
});

app.Run();