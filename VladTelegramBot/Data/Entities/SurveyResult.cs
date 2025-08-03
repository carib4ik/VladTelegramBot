namespace VladTelegramBot.Data.Entities;

public class SurveyResult
{
    public Guid Id { get; set; }
    public required long TelegramId { get; set; }
    public required string TelegramName { get; set; }
    public required long ChatId { get;  set; }
    public string? Answer1 { get; set; }
    public string? Answer2 { get; set; }
    public string? Answer3 { get; set; }
    public string? Answer4 { get; set; }
    public string? Answer5 { get; set; }
    public bool IsPassedTheTest { get;  set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}