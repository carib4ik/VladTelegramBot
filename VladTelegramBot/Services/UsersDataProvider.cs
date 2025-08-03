using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using VladTelegramBot.Data;
using VladTelegramBot.Data.Entities;

namespace VladTelegramBot.Services;

public class UsersDataProvider(AppDbContext dbContext)
{
    private readonly ConcurrentDictionary<long, UserData> _usersData = new();
    private readonly ConcurrentQueue<long> _cash = new();
    
    public async Task<UserData> GetOrCreateUserDataAsync(long chatId, long telegramId = 0, string? telegramName = null)
    {
        TryToClearCash();
        
        if (_usersData.TryGetValue(chatId, out var userData))
        {
            return userData;
        }

        var survey = await dbContext.SurveyResults
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ChatId == chatId);

        if (survey is not null)
        {
            var user =  new UserData
            {
                ChatId = chatId,
                TelegramId = survey.TelegramId,
                TelegramName = survey.TelegramName,
                IsPassedTheTest = true
            };
            
            AddUserToMemory(chatId, user);
            
            return user;
        }

        userData = new UserData
        {
            ChatId = chatId,
            TelegramName = telegramName,
            TelegramId = telegramId
        };
        
        await SaveUserToDatabase(userData);
        
        AddUserToMemory(chatId, userData);
        
        return userData;
    }

    private void AddUserToMemory(long chatId, UserData user)
    {
        _usersData.TryAdd(chatId, user);
        _cash.Enqueue(chatId);
    }

    private async Task SaveUserToDatabase(UserData userData)
    {
        var result = new SurveyResult
        {
            Id = Guid.NewGuid(),
            ChatId = userData.ChatId,
            TelegramName = userData.TelegramName,
            TelegramId = userData.TelegramId,
            SubmittedAt = DateTime.UtcNow
        };

        dbContext.SurveyResults.Add(result);
        await dbContext.SaveChangesAsync();
    }
    
    private void TryToClearCash()
    {
        if (_cash.Count >= 100)
        {
            for (var i = 0; i < 10; i++)
            {
                _cash.TryDequeue(out var oldChatId);
                _usersData.TryRemove(oldChatId, out _);
            }
        }
    }
}