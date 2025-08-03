using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using VladTelegramBot.Data;

namespace VladTelegramBot.StateMachine.States;

public class AdminState(ChatStateMachine stateMachine, ITelegramBotClient botClient) : ChatStateBase(stateMachine)
{
    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }

    public override async Task OnEnter(long chatId)
    {
        var exelButton = InlineKeyboardButton.WithCallbackData("Скачать Exel File", GlobalData.Excel);

        var keyboard = new InlineKeyboardMarkup([[exelButton]]);

        await botClient.SendMessage(chatId, "Меню администратора", replyMarkup: keyboard);

        await StateMachine.TransitTo<IdleState>(chatId);
    }
}