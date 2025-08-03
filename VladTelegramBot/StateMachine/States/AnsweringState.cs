using Telegram.Bot;
using Telegram.Bot.Types;
using VladTelegramBot.Data;
using VladTelegramBot.Data.Entities;
using VladTelegramBot.Services;

namespace VladTelegramBot.StateMachine.States;

public class AnsweringState(ChatStateMachine stateMachine, ITelegramBotClient botClient, UsersDataProvider usersDataProvider, AppDbContext dbContext)
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
        Console.WriteLine("Answering state");
        
        await botClient.SendMessage(chatId, GlobalData.Question1);
    }

    public override async Task OnExit(long chatId)
    {
        var userData = await usersDataProvider.GetOrCreateUserDataAsync(chatId);

        if (userData.IsPassedTheTest)
        {
            var result = new SurveyResult
            {
                Id = Guid.NewGuid(),
                ChatId = chatId,
                TelegramName = userData.TelegramName,
                TelegramId = userData.TelegramId,
                Answer1 = userData.Answer1,
                Answer2 = userData.Answer2,
                Answer3 = userData.Answer3,
                Answer4 = userData.Answer4,
                Answer5 = userData.Answer5,
                IsPassedTheTest = userData.IsPassedTheTest,
                SubmittedAt = DateTime.UtcNow
            };

            dbContext.SurveyResults.Add(result);
            await dbContext.SaveChangesAsync();
        }
        
        await base.OnExit(chatId);
    }
}