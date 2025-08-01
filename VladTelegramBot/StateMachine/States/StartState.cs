using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using VladTelegramBot.Data;
using VladTelegramBot.Services;

namespace VladTelegramBot.StateMachine.States;

public class StartState(ChatStateMachine stateMachine, ITelegramBotClient botClient, UsersDataProvider usersDataProvider)
    : ChatStateBase(stateMachine)
{
    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }

    public override async Task OnEnter(long chatId)
    {
        await SendGreeting(chatId);
    }

    private async Task SendGreeting(long chatId)
    {
        if (CheckUserPassedTheTest(chatId))
        {
            var joinButton = InlineKeyboardButton.WithCallbackData("Вступить в закрытый канал", GlobalData.Join);

            var keyboard = new InlineKeyboardMarkup([[joinButton]]);

            await botClient.SendMessage(chatId, GlobalData.GreetingsAfterTest, replyMarkup: keyboard);
        
            await StateMachine.TransitTo<IdleState>(chatId);
        }
        else
        {
            var answerButton = InlineKeyboardButton.WithCallbackData("Start answering", GlobalData.Answer);

            var keyboard = new InlineKeyboardMarkup([[answerButton]]);

            await botClient.SendMessage(chatId, GlobalData.GreetingsBeforeTest, replyMarkup: keyboard);
        
            await StateMachine.TransitTo<IdleState>(chatId);
        }
    }

    private bool CheckUserPassedTheTest(long chatId)
    {
        return usersDataProvider.GetUserData(chatId).IsPassedTheTest;
    }
}