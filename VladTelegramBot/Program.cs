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
                // Автоматическое биндинг AppConfig из IConfiguration
                services.Configure<AppConfig>(context.Configuration);
                services.AddSingleton(sp => sp.GetRequiredService<IOptions<AppConfig>>().Value);

                var config = context.Configuration.Get<AppConfig>();
                
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
}