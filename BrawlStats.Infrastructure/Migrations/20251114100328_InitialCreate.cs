using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrawlStats.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brawlers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrawlerId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brawlers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerTag = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Trophies = table.Column<int>(type: "int", nullable: false),
                    HighestTrophies = table.Column<int>(type: "int", nullable: false),
                    ExpLevel = table.Column<int>(type: "int", nullable: false),
                    ExpPoints = table.Column<int>(type: "int", nullable: false),
                    Victories3v3 = table.Column<int>(type: "int", nullable: false),
                    SoloVictories = table.Column<int>(type: "int", nullable: false),
                    DuoVictories = table.Column<int>(type: "int", nullable: false),
                    ClubTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClubName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetaSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SnapshotDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BrawlerId = table.Column<int>(type: "int", nullable: false),
                    BrawlerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PickRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WinRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalGames = table.Column<int>(type: "int", nullable: false),
                    TotalWins = table.Column<int>(type: "int", nullable: false),
                    Tier = table.Column<int>(type: "int", nullable: false),
                    TrendDirection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrendPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Mode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Map = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetaSnapshots_Brawlers_BrawlerId",
                        column: x => x.BrawlerId,
                        principalTable: "Brawlers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Battles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BattleTime = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BattleDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Mode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Map = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Result = table.Column<int>(type: "int", nullable: false),
                    TrophyChange = table.Column<int>(type: "int", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: true),
                    IsStarPlayer = table.Column<bool>(type: "bit", nullable: false),
                    PlayerTag = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    BrawlerId = table.Column<int>(type: "int", nullable: false),
                    BrawlerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrawlerPower = table.Column<int>(type: "int", nullable: false),
                    BrawlerTrophies = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Battles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Battles_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    PlayerTag = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SkillRating = table.Column<int>(type: "int", nullable: false),
                    ConsistencyScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClutchRating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImprovementTrend = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Last10Wins = table.Column<int>(type: "int", nullable: false),
                    Last10Losses = table.Column<int>(type: "int", nullable: false),
                    Last10WinRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FavoriteMode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FavoriteBrawler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BestTimeToPlay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastCalculated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerAnalytics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerAnalytics_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerBrawlers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    PlayerTag = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BrawlerId = table.Column<int>(type: "int", nullable: false),
                    BrawlerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Power = table.Column<int>(type: "int", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    Trophies = table.Column<int>(type: "int", nullable: false),
                    HighestTrophies = table.Column<int>(type: "int", nullable: false),
                    TotalBattles = table.Column<int>(type: "int", nullable: false),
                    Wins = table.Column<int>(type: "int", nullable: false),
                    Losses = table.Column<int>(type: "int", nullable: false),
                    WinRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerBrawlers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerBrawlers_Brawlers_BrawlerId",
                        column: x => x.BrawlerId,
                        principalTable: "Brawlers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerBrawlers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerTrophyHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    PlayerTag = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Trophies = table.Column<int>(type: "int", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerTrophyHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerTrophyHistory_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Battles_BattleDateTime",
                table: "Battles",
                column: "BattleDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_PlayerId",
                table: "Battles",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_PlayerTag_BattleTime",
                table: "Battles",
                columns: new[] { "PlayerTag", "BattleTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Brawlers_BrawlerId",
                table: "Brawlers",
                column: "BrawlerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MetaSnapshots_BrawlerId_SnapshotDate_Mode",
                table: "MetaSnapshots",
                columns: new[] { "BrawlerId", "SnapshotDate", "Mode" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAnalytics_PlayerId",
                table: "PlayerAnalytics",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAnalytics_PlayerTag",
                table: "PlayerAnalytics",
                column: "PlayerTag",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerBrawlers_BrawlerId",
                table: "PlayerBrawlers",
                column: "BrawlerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerBrawlers_PlayerId",
                table: "PlayerBrawlers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerBrawlers_PlayerTag_BrawlerId",
                table: "PlayerBrawlers",
                columns: new[] { "PlayerTag", "BrawlerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_PlayerTag",
                table: "Players",
                column: "PlayerTag",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTrophyHistory_PlayerId",
                table: "PlayerTrophyHistory",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTrophyHistory_PlayerTag_RecordedAt",
                table: "PlayerTrophyHistory",
                columns: new[] { "PlayerTag", "RecordedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Battles");

            migrationBuilder.DropTable(
                name: "MetaSnapshots");

            migrationBuilder.DropTable(
                name: "PlayerAnalytics");

            migrationBuilder.DropTable(
                name: "PlayerBrawlers");

            migrationBuilder.DropTable(
                name: "PlayerTrophyHistory");

            migrationBuilder.DropTable(
                name: "Brawlers");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
