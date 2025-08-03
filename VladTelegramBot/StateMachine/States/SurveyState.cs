using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using VladTelegramBot.Data;
using VladTelegramBot.Services;

namespace VladTelegramBot.StateMachine.States;

public class SurveyState(
    ChatStateMachine stateMachine,
    ITelegramBotClient botClient,
    UsersDataProvider usersDataProvider,
    AppDbContext dbContext)
    : ChatStateBase(stateMachine)
{
    public override async Task HandleMessage(Message message)
    {
        var chatId = message.Chat.Id;
        var userAnswer = message.Text;

        var userData = await usersDataProvider.GetOrCreateUserDataAsync(chatId);

        switch (userData.SurveyStep)
        {
            case 1:
                userData.Answer1 = userAnswer;
                await botClient.SendMessage(chatId, GlobalData.Question2);
                userData.SurveyStep++;
                break;
            case 2:
                userData.Answer2 = userAnswer;
                await botClient.SendMessage(chatId, GlobalData.Question3);
                userData.SurveyStep++;
                break;
            case 3:
                userData.Answer3 = userAnswer;
                await botClient.SendMessage(chatId, GlobalData.Question4);
                userData.SurveyStep++;
                break;
            case 4:
                userData.Answer4 = userAnswer;
                await botClient.SendMessage(chatId, GlobalData.Question5);
                userData.SurveyStep++;
                break;
            case 5:
                userData.Answer5 = userAnswer;
                userData.SurveyStep = 1;
                userData.IsPassedTheTest = true;
                await StateMachine.TransitTo<StartState>(chatId);
                break;
        }
    }

    public override async Task OnEnter(long chatId)
    {
        Console.WriteLine("Survey state");

        await botClient.SendMessage(chatId, GlobalData.Question1);
    }

    public override async Task OnExit(long chatId)
    {
        await TryToSaveUserAnswersToDb(chatId);
        await base.OnExit(chatId);
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