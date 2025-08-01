using Telegram.Bot;
using Telegram.Bot.Types;
using VladTelegramBot.Data;
using VladTelegramBot.Services;

namespace VladTelegramBot.StateMachine.States;

public class AnsweringState(ChatStateMachine stateMachine, ITelegramBotClient botClient, UsersDataProvider usersDataProvider)
    : ChatStateBase(stateMachine)
{
    public override async Task HandleMessage(Message message)
    {
        var chatId = message.Chat.Id;
        var userData = usersDataProvider.GetUserData(chatId);
        var userAnswer = message.Text;
        
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
}