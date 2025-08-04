using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder
                    .AddJsonFile("AppConfigs/appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;

                var config = new AppConfig
                {
                    TelegramBotToken = configuration["TelegramBotToken"] ?? throw new InvalidOperationException("Missing bot token"),
                    ConnectionString = configuration["ConnectionString"] ?? throw new InvalidOperationException("Missing connection string"),
                    AdminTelegramIds = ParseAdminIds(configuration["AdminTelegramIds"])
                };
                
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
    
    private static List<long> ParseAdminIds(string? ids)
    {
        return ids?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(id => long.TryParse(id.Trim(), out var parsed) ? parsed : 0)
            .Where(id => id > 0)
            .ToList() ?? new List<long>();
    }
}