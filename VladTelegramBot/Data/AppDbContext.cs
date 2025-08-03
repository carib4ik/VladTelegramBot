using Microsoft.EntityFrameworkCore;
using VladTelegramBot.Data.Entities;

namespace VladTelegramBot.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserDataEntity> SurveyResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserDataEntity>()
            .HasIndex(x => x.ChatId)
            .IsUnique();
    }
}