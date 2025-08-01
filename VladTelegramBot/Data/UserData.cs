namespace VladTelegramBot.Data;

public class UserData
{
    public string? TelegramName { get;  set; }
    public long? ChatId { get;  set; }
    public bool IsPassedTheTest { get;  set; }
    
    public int SurveyStep { get; set; } = 1;
    public string? Answer1 { get; set; }
    public string? Answer2 { get; set; }
    public string? Answer3 { get; set; }
    public string? Answer4 { get; set; }
    public string? Answer5 { get; set; }
}