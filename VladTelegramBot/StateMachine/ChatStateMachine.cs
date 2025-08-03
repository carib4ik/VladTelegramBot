using System.Collections.Concurrent;
using Telegram.Bot;
using VladTelegramBot.AppConfigs;
using VladTelegramBot.Data;
using VladTelegramBot.Services;
using VladTelegramBot.StateMachine.States;

namespace VladTelegramBot.StateMachine;

public class ChatStateMachine
{
    private readonly ConcurrentDictionary<long, ChatStateBase> _chatStates = new();
    private readonly Dictionary<Type, Func<ChatStateBase>> _states = new();

    public ChatStateMachine(ITelegramBotClient botClient, UsersDataProvider usersDataProvider, AppDbContext dbContext,
        AppConfig appConfig)
    {
        _states[typeof(IdleState)] = () => new IdleState(this);
        _states[typeof(StartState)] = () => new StartState(this, botClient, usersDataProvider, appConfig);
        _states[typeof(SurveyState)] = () => new SurveyState(this, botClient, usersDataProvider);
        _states[typeof(UserDataSubmissionState)] = () => new UserDataSubmissionState(this, usersDataProvider, dbContext);
        _states[typeof(InviteState)] = () => new InviteState(this, botClient);
        _states[typeof(AdminState)] = () => new AdminState(this, botClient);
    }

    public ChatStateBase GetState(long chatId)
    {
        return !_chatStates.TryGetValue(chatId, out var state) ? _states[typeof(IdleState)].Invoke() : state;
    }

    public async Task TransitTo<T>(long chatId) where T : ChatStateBase
    {
        if (_chatStates.TryGetValue(chatId, out var currentState))
        {
            await currentState.OnExit(chatId);
        }

        var newState = _states[typeof(T)]();
        _chatStates[chatId] = newState;
        await newState.OnEnter(chatId);
    }
}