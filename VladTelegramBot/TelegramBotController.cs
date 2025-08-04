using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using VladTelegramBot.Data;
using VladTelegramBot.Services;

namespace VladTelegramBot;

public class TelegramBotController(
    ITelegramBotClient botClient,
    ChatStateController chatStateController,
    UsersDataProvider usersDataProvider)
{
    public void StartBot()
    {
        using var cts = new CancellationTokenSource();
        
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery]
        };

        CreateCommandsKeyboard().WaitAsync(cts.Token);
        
        botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken: cts.Token);

        Console.WriteLine("Bot started");
    }

    private async Task CreateCommandsKeyboard()
    {
        await botClient.DeleteMyCommands();

        var commands = new[]
        {
            new BotCommand { Command = GlobalData.Start, Description = "Запустить бота" }
        };
        
        await botClient.SetMyCommands(
            commands,
            scope: new BotCommandScopeDefault(),
            languageCode: "ru"
        );
    }
    
    private Task HandleErrorAsync(ITelegramBotClient telegramBotClient, Exception exception,
        CancellationToken cancellationToken)
    {
        var requestException = exception switch
        {
            ApiRequestException apiRequestException => apiRequestException,
            _ => exception
        };

        Console.WriteLine("Произошла критическая ошибка. Требуется *ПЕРЕЗАПУСК* бота\n" + requestException.Message);
        return Task.CompletedTask;
    }
    
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, 
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Update received: {update.Type}");
        
        if (update.Message == null && update.CallbackQuery == null)
        {
            return;
        }
        
        var message = update.Message;
        var callbackQuery = update.CallbackQuery;
        
        if (message != null && message.Type != MessageType.Text)
        {
            return;
        }
        
        var messageId = message != null ? message.MessageId : callbackQuery.Message.MessageId;
        var messageText = message != null ? message.Text : callbackQuery?.Data;
        var chatId = message != null ? message.Chat.Id : callbackQuery.Message.Chat.Id;
        var telegramName = message != null ? message.From.Username : callbackQuery.From.Username;

        await botClient.SendChatAction(chatId, ChatAction.Typing, cancellationToken: cancellationToken);
        await usersDataProvider.GetOrCreateUserDataAsync(chatId, telegramName);

        if (messageText == GlobalData.Start || messageText == GlobalData.Answer)
        {
            await DeleteMessageAsync(chatId, messageId, cancellationToken);
        }
        
        await chatStateController.HandleUpdateAsync(update);
    }

    
    private async Task DeleteMessageAsync(long chatId, int messageId, CancellationToken cancellationToken)
    {
        try
        {
            await botClient.DeleteMessage(chatId, messageId, cancellationToken: cancellationToken);
        }
        catch (ApiRequestException exception)
        {
            Console.WriteLine(exception.Message);
        }
    }
}