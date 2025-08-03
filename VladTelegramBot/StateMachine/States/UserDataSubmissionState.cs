using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using VladTelegramBot.Data;
using VladTelegramBot.Services;

namespace VladTelegramBot.StateMachine.States;

public class UserDataSubmissionState(
    ChatStateMachine stateMachine,
    UsersDataProvider usersDataProvider,
    AppDbContext dbContext) 
    : ChatStateBase(stateMachine)
{
    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }

    public override async Task OnEnter(long chatId)
    {
        await TryToSaveUserAnswersToDb(chatId);
        await StateMachine.TransitTo<IdleState>(chatId);
    }

    private async Task TryToSaveUserAnswersToDb(long chatId)
    {
        var userData = await usersDataProvider.GetOrCreateUserDataAsync(chatId);

        if (userData.IsPassedTheTest)
        {
            var existingResult = await dbContext.SurveyResults
                .FirstOrDefaultAsync(x => x.ChatId == chatId);

            if (existingResult != null)
            {
                existingResult.Answer1 = userData.Answer1;
                existingResult.Answer2 = userData.Answer2;
                existingResult.Answer3 = userData.Answer3;
                existingResult.Answer4 = userData.Answer4;
                existingResult.Answer5 = userData.Answer5;
                existingResult.IsPassedTheTest = userData.IsPassedTheTest;

                await dbContext.SaveChangesAsync();

                Console.WriteLine("Ответы сохранены в базе");
            }
            else
            {
                Console.WriteLine("Ошибка обновления данных после опроса");
            }
        }
    }
}