using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using VladTelegramBot.Data;
using VladTelegramBot.Services;

namespace VladTelegramBot.StateMachine.States;

public class SurveyState(
    ChatStateMachine stateMachine,
    ITelegramBotClient botClient,
    UsersDataProvider usersDataProvider)
    : ChatStateBase(stateMachine)
{
    public override async Task HandleMessage(Message message, CallbackQuery? callbackQuery = null)
    {
        var chatId = message.Chat.Id;
        var userAnswer = message.Text;

        var userData = await usersDataProvider.GetOrCreateUserDataAsync(chatId);

        switch (userData.SurveyStep)
        {
            case 1:
                userData.Answer1 = userAnswer;
                await botClient.SendMessage(chatId, GlobalData.Question2);
                userData.SurveyStep++;
                break;
            case 2:
                userData.Answer2 = userAnswer;
                await botClient.SendMessage(chatId, GlobalData.Question3, replyMarkup: CreateKeyBoardForQuestion3());
                userData.SurveyStep++;
                break;
            case 3:
                userData.Answer3 = callbackQuery != null ? callbackQuery.Data : userAnswer;
                await botClient.SendMessage(chatId, GlobalData.Question4);
                userData.SurveyStep++;
                break;
            case 4:
                userData.Answer4 = userAnswer;
                await botClient.SendMessage(chatId, GlobalData.Question5);
                userData.SurveyStep++;
                break;
            case 5:
                userData.Answer5 = userAnswer;
                userData.SurveyStep = 1;
                userData.IsPassedTheTest = true;
                await StateMachine.TransitTo<UserDataSubmissionState>(chatId);
                break;
        }
    }

    public override async Task OnEnter(long chatId)
    {
        Console.WriteLine("Survey state");

        await botClient.SendMessage(chatId, GlobalData.Question1);
    }

    private InlineKeyboardMarkup CreateKeyBoardForQuestion3()
    {
        var answerButton1 = InlineKeyboardButton.WithCallbackData("Открыть бизнес", "Открыть бизнес");
        var answerButton2 = InlineKeyboardButton.WithCallbackData("Масштабирование", "Масштабирование");
        var answerButton3 = InlineKeyboardButton.WithCallbackData("Увеличить прибыль", "Увеличить прибыль");
        var answerButton4 = InlineKeyboardButton.WithCallbackData("Команда", "Команда");
        var answerButton5 = InlineKeyboardButton.WithCallbackData("Автоматизация бизнес процессов", "Автоматизация бизнес процессов");

        var keyboard = new InlineKeyboardMarkup([
            [answerButton1, answerButton2], [answerButton3, answerButton4], [answerButton5]
        ]);

        return keyboard;
    }
}
