using Telegram.Bot.Types;

namespace VladTelegramBot.StateMachine.States;

public class IdleState(ChatStateMachine stateMachine) : ChatStateBase(stateMachine)
{
    public override Task OnEnter(long chatId)
    {
        Console.WriteLine("IdleState");
        return Task.CompletedTask;
    }

    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }
}