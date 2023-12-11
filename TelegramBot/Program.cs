using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using System.Threading;
using Microsoft.IdentityModel.Tokens;
using TelegramBot.BD;
using static System.Runtime.InteropServices.JavaScript.JSType;

class Program
{
    
    static void Main(string[] args)
    {
        var client = new TelegramBotClient("6732493440:AAGgyzhTGhjzc5YVO07sIaCNb6ksbMA4gcU");
        client.StartReceiving(Update, Error);
        Console.ReadLine();
    }

    private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }

    async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        SubscriberTelegramBot SubscriberTelegramBot;
        var message = update.Message;

        await botClient.SendTextMessageAsync(
        chatId: message.Chat.Id,
        text: "Что сказал?");
        var findChatID = AcsContext.GetInstance().SubscriberTelegramBots.FirstOrDefault(s => s.ChatId == message.Chat.Id);
        if (!message.Text.IsNullOrEmpty())
        {
            if (message.Text.Contains("Хочу получать уведомления, 12345") && findChatID == null)
            {
                SubscriberTelegramBot = new SubscriberTelegramBot() { ChatId = (int)message.Chat.Id };
                AcsContext.GetInstance().SubscriberTelegramBots.Add(SubscriberTelegramBot);
                AcsContext.GetInstance().SaveChanges();
            }
        }
    }
}
