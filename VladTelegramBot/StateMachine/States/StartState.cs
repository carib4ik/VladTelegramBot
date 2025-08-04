using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using VladTelegramBot.AppConfigs;
using VladTelegramBot.Data;
using VladTelegramBot.Services;

namespace VladTelegramBot.StateMachine.States;

public class StartState(
    ChatStateMachine stateMachine,
    ITelegramBotClient botClient,
    UsersDataProvider usersDataProvider,
    AppConfig appConfig)
    : ChatStateBase(stateMachine)
{
    public override Task HandleMessage(Message message, CallbackQuery? callbackQuery = null)
    {
        return Task.CompletedTask;
    }

    public override async Task OnEnter(long chatId)
    {
        Console.WriteLine("Start state");
        await SendGreeting(chatId);
    }

    private async Task SendGreeting(long chatId)
    {
        if (IsUserAdmin(chatId))
        {
            await StateMachine.TransitTo<AdminState>(chatId);
            return;
        }
        
        if (HasUserPassedTheTest(chatId))
        {
            await StateMachine.TransitTo<InviteState>(chatId);
        }
        else
        {
            var answerButton = InlineKeyboardButton.WithCallbackData("Пройти опрос", GlobalData.Answer);

            var keyboard = new InlineKeyboardMarkup([[answerButton]]);

            await botClient.SendMessage(chatId, GlobalData.GreetingsBeforeTest, replyMarkup: keyboard);

            await StateMachine.TransitTo<IdleState>(chatId);
        }
    }

    private bool HasUserPassedTheTest(long chatId)
    {
        return usersDataProvider.GetOrCreateUserDataAsync(chatId).Result.IsPassedTheTest;
    }
    
    private bool IsUserAdmin(long chatId)
    {
        return appConfig.AdminTelegramIds.Any(adminId => adminId == chatId);
    }
}