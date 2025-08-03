namespace VladTelegramBot.Data.Entities;

public class UserDataEntity
{
    public Guid Id { get; set; }
    public long ChatId { get;  set; }
    public string? TelegramName { get; set; }
    public string? Answer1 { get; set; }
    public string? Answer2 { get; set; }
    public string? Answer3 { get; set; }
    public string? Answer4 { get; set; }
    public string? Answer5 { get; set; }
    public bool IsPassedTheTest { get;  set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}