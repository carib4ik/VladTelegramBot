using Microsoft.EntityFrameworkCore;
using VladTelegramBot.Data.Entities;

namespace VladTelegramBot.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<SurveyResult> SurveyResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SurveyResult>()
            .HasIndex(x => x.TelegramId)
            .IsUnique();
    }
}