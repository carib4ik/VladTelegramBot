using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using VladTelegramBot.Data;
using VladTelegramBot.Data.Entities;

namespace VladTelegramBot.Services;

public class UsersDataProvider(AppDbContext dbContext)
{
    private readonly ConcurrentDictionary<long, UserData> _usersData = new();
    
    public async Task<UserData> GetOrCreateUserDataAsync(long chatId, long telegramId = 0, string? telegramName = null)
    {
        if (_usersData.TryGetValue(chatId, out var userData))
        {
            Console.WriteLine($"User is found in the InMemory. TelegramName {userData.TelegramName}, TelegramId {userData.TelegramId}, passed the test {userData.IsPassedTheTest}");
            return userData;
        }

        var survey = await dbContext.SurveyResults
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ChatId == chatId);

        if (survey is not null)
        {
            Console.WriteLine($"User is found in the DB. TelegramName {survey.TelegramName}, TelegramId {survey.TelegramId}, passed the test {survey.IsPassedTheTest}");
            
            var user =  new UserData
            {
                ChatId = chatId,
                TelegramId = survey.TelegramId,
                TelegramName = survey.TelegramName,
                IsPassedTheTest = true
            };
            
            _usersData.TryAdd(chatId, user);
            return user;
        }

        userData = new UserData
        {
            ChatId = chatId,
            TelegramName = telegramName,
            TelegramId = telegramId
        };
        
        var result = new SurveyResult
        {
            Id = Guid.NewGuid(),
            ChatId = chatId,
            TelegramName = userData.TelegramName,
            TelegramId = userData.TelegramId,
            SubmittedAt = DateTime.UtcNow
        };

        dbContext.SurveyResults.Add(result);
        await dbContext.SaveChangesAsync();
        
        Console.WriteLine($"User has been saved. TelegramName {userData.TelegramName}, TelegramId {userData.TelegramId}, passed the test {userData.IsPassedTheTest}");
        
        _usersData.TryAdd(chatId, userData);
        
        return userData;
    }

    public void RemoveUserFromInMemory(long chatId)
    {
        _usersData.TryRemove(chatId, out _);
    }

    public bool HasUserPassedSurvey(long chatId)
    {
        return _usersData.TryGetValue(chatId, out var data) && data.IsPassedTheTest;
    }

    public IEnumerable<UserData> GetAll() => _usersData.Values;
}