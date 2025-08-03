using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using VladTelegramBot.AppConfigs;
using VladTelegramBot.Data;
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
                
                services.AddScoped<UsersDataProvider>();
                services.AddScoped<ExcelExportService>();
                services.AddScoped<ChatStateMachine>();
                services.AddScoped<ChatStateController>();
                services.AddScoped<TelegramBotController>();

                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(config.ConnectionString));
            })
            .Build();
        
        var bot = host.Services.GetRequiredService<TelegramBotController>();
        bot.StartBot();

        await host.RunAsync();
    }
    
    private static AppConfig LoadConfig()
    {
        var json = File.ReadAllText("AppConfigs/appsettings.json");
        return JsonSerializer.Deserialize<AppConfig>(json)!;
    }
}