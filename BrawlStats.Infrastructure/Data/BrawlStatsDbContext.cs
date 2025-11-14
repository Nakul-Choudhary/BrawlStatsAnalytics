using Microsoft.EntityFrameworkCore;
using BrawlStats.Domain.Entities;

namespace BrawlStats.Infrastructure.Data
{
    public class BrawlStatsDbContext : DbContext
    {
        public BrawlStatsDbContext(DbContextOptions<BrawlStatsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Battle> Battles { get; set; }
        public DbSet<Brawler> Brawlers { get; set; }
        public DbSet<PlayerBrawler> PlayerBrawlers { get; set; }
        public DbSet<PlayerTrophyHistory> PlayerTrophyHistory { get; set; }
        public DbSet<MetaSnapshot> MetaSnapshots { get; set; }
        public DbSet<PlayerAnalytics> PlayerAnalytics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Player configuration
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.PlayerTag).IsUnique();
                entity.Property(e => e.PlayerTag).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Battle configuration
            modelBuilder.Entity<Battle>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.PlayerTag, e.BattleTime });
                entity.HasIndex(e => e.BattleDateTime);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Player)
                    .WithMany(p => p.Battles)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Brawler configuration
            modelBuilder.Entity<Brawler>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.BrawlerId).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            });

            // PlayerBrawler configuration
            modelBuilder.Entity<PlayerBrawler>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.PlayerTag, e.BrawlerId }).IsUnique();

                entity.HasOne(e => e.Player)
                    .WithMany(p => p.PlayerBrawlers)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Brawler)
                    .WithMany(b => b.PlayerBrawlers)
                    .HasForeignKey(e => e.BrawlerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PlayerTrophyHistory configuration
            modelBuilder.Entity<PlayerTrophyHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.PlayerTag, e.RecordedAt });

                entity.HasOne(e => e.Player)
                    .WithMany(p => p.TrophyHistory)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // MetaSnapshot configuration
            modelBuilder.Entity<MetaSnapshot>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.BrawlerId, e.SnapshotDate, e.Mode });
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Brawler)
                    .WithMany(b => b.MetaSnapshots)
                    .HasForeignKey(e => e.BrawlerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // PlayerAnalytics configuration
            modelBuilder.Entity<PlayerAnalytics>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.PlayerTag).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Player)
                    .WithOne()
                    .HasForeignKey<PlayerAnalytics>(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}