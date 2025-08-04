using Telegram.Bot;
using Telegram.Bot.Types;
using VladTelegramBot.Services;

namespace VladTelegramBot.StateMachine.States;

public class SendExelState(ChatStateMachine stateMachine, ITelegramBotClient botClient, ExcelExportService excelExportService)
    : ChatStateBase(stateMachine)
{
    public override Task HandleMessage(Message message, CallbackQuery? callbackQuery = null)
    {
        return Task.CompletedTask;
    }

    public override async Task OnEnter(long chatId)
    {
        var excelBytes = await excelExportService.ExportSurveyResultsToExcelAsync();
        
        using var stream = new MemoryStream(excelBytes);
        
        await botClient.SendDocument(
            chatId: chatId,
            document: new InputFileStream(stream, "survey_results.xlsx"),
            caption: "Вот Excel-файл с результатами опроса"
        );
    }
}