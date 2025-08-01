using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using VladTelegramBot.Services;
using VladTelegramBot.StateMachine;

namespace VladTelegramBot;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var config = LoadConfig();
        
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton(config);
                services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(config.TelegramBotToken));
                services.AddSingleton<UsersDataProvider>();
                services.AddSingleton<ChatStateMachine>();
                services.AddSingleton<ChatStateController>();
                services.AddSingleton<TelegramBotController>();
            })
            .Build();
        
        var bot = host.Services.GetRequiredService<TelegramBotController>();
        bot.StartBot();

        await host.RunAsync();
    }
    
    private static AppConfig.AppConfig LoadConfig()
    {
        return new AppConfig.AppConfig
        {
            TelegramBotToken = "8051065649:AAGS1okoQfC4y3ZEMPwdqkRILIJn00G-6mk",
        };
    }
}