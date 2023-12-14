using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using System.Threading;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;
using LibraryBD.BD;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

class Program
{
    static HttpClient httpClient = new HttpClient();
    static async Task Main(string[] args)
    {
        var client = new TelegramBotClient("6732493440:AAGgyzhTGhjzc5YVO07sIaCNb6ksbMA4gcU");
        client.StartReceiving(Update, Error);
        Console.ReadLine();
    }

    async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        SubscriberTelegramBot SubscriberTelegramBot;
        var message = update.Message;

        await botClient.SendTextMessageAsync(
        chatId: message.Chat.Id,
        text: message.Text);

        try
        {
            var findChatID = AcsContext.GetInstance().SubscriberTelegramBots.FirstOrDefault(s => s.ChatId == message.Chat.Id);
            if (!message.Text.IsNullOrEmpty())
            {
                if (message.Text.Contains("Хочу получать уведомления, 12345") && findChatID == null)
                {
                    SubscriberTelegramBot = new SubscriberTelegramBot()
                    {
                        ChatId = (int)message.Chat.Id,
                        Username = message.Chat.Username,
                        Name = message.Chat.FirstName,
                        Surname = message.Chat.LastName
                    };

                    AcsContext.GetInstance().SubscriberTelegramBots.Add(SubscriberTelegramBot);
                    AcsContext.GetInstance().SaveChanges();
                }
            }
        }
        catch
        {
            Console.WriteLine("Ошибка подписки!");
            Console.ReadLine();
        }
        
        try
        {
            while (true)
            {
                List<Offender> checkOffender = await httpClient.GetFromJsonAsync<List<Offender>>("https://localhost:7123/api/Offenders/GetOffenders");
                if (checkOffender != null)
                {
                    foreach (var offender in checkOffender)
                    {
                        List<SubscriberTelegramBot>? Subscribers = await httpClient.GetFromJsonAsync<List<SubscriberTelegramBot>> ("https://localhost:7123/api/SubscriberTelegramBots/GetSubscriberTelegramBots");

                        foreach (var subscriber in Subscribers)
                        {
                            if (offender.SendOrNot == 0 && subscriber.SubscribeOrNot == 1)
                            {
                                message.Chat.Id = subscriber.ChatId;

                                await botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: $"Нарушитель:\n{offender.Name}\n{offender.Position}\n{offender.Time}");
                            }

                        }
                        var offenderSendApi = await httpClient.PostAsJsonAsync("https://localhost:7123/api/Offenders/SendOrNot", offender);
                    }
                }
                
                return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
            Console.ReadLine();
        }
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
}
