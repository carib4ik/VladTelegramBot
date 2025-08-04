using Telegram.Bot.Types;

namespace VladTelegramBot.StateMachine.States;

public abstract class ChatStateBase(ChatStateMachine stateMachine)
{
    protected readonly ChatStateMachine StateMachine = stateMachine;

    public abstract Task HandleMessage(Message message, CallbackQuery? callbackQuery = null);
    
    public virtual Task OnEnter(long chatId)
    {
        return Task.CompletedTask;
    }
    
    public virtual Task OnExit(long chatId)
    {
        return Task.CompletedTask;
    }
}