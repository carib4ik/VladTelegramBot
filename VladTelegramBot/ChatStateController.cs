using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using VladTelegramBot.Data;
using VladTelegramBot.StateMachine;
using VladTelegramBot.StateMachine.States;

namespace VladTelegramBot;

public class ChatStateController(ChatStateMachine chatStateMachine)
{
    public async Task HandleUpdateAsync(Update update)
    {
        if (update.Message == null && update.CallbackQuery == null)
        {
            return;
        }

        string? data;
        Message message;
        CallbackQuery? callbackQuery = null;
        
        switch (update.Type)
        {
            case UpdateType.Message:
                data = update.Message.Text;
                message = update.Message;
                break;
            
            case UpdateType.CallbackQuery:
                data = update.CallbackQuery.Data;
                message = update.CallbackQuery.Message;
                callbackQuery = update.CallbackQuery;
                break;
            
            default:
                return;
        }
        
        var chatId = message.Chat.Id;
        
        switch (data)
        {
            case GlobalData.Start:
                await chatStateMachine.TransitTo<StartState>(chatId);
                break;
            
            case GlobalData.Answer:
                await chatStateMachine.TransitTo<SurveyState>(chatId);
                break;
            
            case GlobalData.Excel:
                await chatStateMachine.TransitTo<SendExelState>(chatId);
                break;
            
            default:
                var state = chatStateMachine.GetState(chatId);
                await state.HandleMessage(message, callbackQuery);
                break;
        }
    }
}