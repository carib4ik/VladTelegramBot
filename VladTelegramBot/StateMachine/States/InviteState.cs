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
    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }

    public override async Task OnEnter(long chatId)
    {
        var joinButton = InlineKeyboardButton.WithCallbackData("Вступить в закрытый канал", GlobalData.Join);

        var keyboard = new InlineKeyboardMarkup([[joinButton]]);

        await botClient.SendMessage(chatId, GlobalData.GreetingsAfterTest, replyMarkup: keyboard);

        await StateMachine.TransitTo<IdleState>(chatId);
    }
    
}