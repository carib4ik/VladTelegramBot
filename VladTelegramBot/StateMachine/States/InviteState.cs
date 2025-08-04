using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using VladTelegramBot.AppConfigs;
using VladTelegramBot.Data;

namespace VladTelegramBot.StateMachine.States;

public class InviteState(
    ChatStateMachine stateMachine,
    ITelegramBotClient  botClient) 
    : ChatStateBase(stateMachine)
{
    public override Task HandleMessage(Message message, CallbackQuery? callbackQuery = null)
    {
        return Task.CompletedTask;
    }

    public override async Task OnEnter(long chatId)
    {
        var joinButton = InlineKeyboardButton.WithUrl("Вступить в закрытый канал", GlobalData.Join);

        var keyboard = new InlineKeyboardMarkup([[joinButton]]);

        await botClient.SendMessage(chatId, GlobalData.GreetingsAfterTest, replyMarkup: keyboard);

        await StateMachine.TransitTo<IdleState>(chatId);
    }
    
}