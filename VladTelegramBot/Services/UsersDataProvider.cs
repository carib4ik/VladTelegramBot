using System.Collections.Concurrent;
using VladTelegramBot.Data;

namespace VladTelegramBot.Services;

public class UsersDataProvider
{
    private readonly ConcurrentDictionary<long, UserData> _usersData = new();
    
    public void SetTelegramName(long chatId, string? telegramName)
    {
        var userData = _usersData.GetOrAdd(chatId, new UserData());
        userData.TelegramName = telegramName;
        userData.ChatId =  chatId;
    }
    
    public UserData GetUserData(long chatId)
    {
        return _usersData[chatId];
    }
}