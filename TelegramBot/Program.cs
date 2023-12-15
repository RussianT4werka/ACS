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
using System.Net.Http;
using NuGet.Common;
using Update = Telegram.Bot.Types.Update;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

class Program
{
    static HttpClient httpClient = new HttpClient();
    private static List<Offender> ListOffenders { get; set; }

    static ITelegramBotClient BotClient;
    static Update Update;
    static CancellationToken Token;

    static async Task Main(string[] args)
    {

        var client = new TelegramBotClient("6732493440:AAGgyzhTGhjzc5YVO07sIaCNb6ksbMA4gcU");
        //client.StartReceiving(UpdateM, Error);

        client.StartReceiving(CheckOffender, Error);

        var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        while (await timer.WaitForNextTickAsync())
        {

            CheckOffender(BotClient, Update, Token);
        }


        Console.ReadLine();
    }
    public static SubscriberTelegramBot ChatID { get; set; }
    async static Task UpdateM(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        
        SubscriberTelegramBot SubscriberTelegramBot;
        var message = update.Message;
        
        /*try
        {
            var chatID = Convert.ToInt32(message.Chat.Id);
            var check = await httpClient.GetFromJsonAsync<int>($"https://localhost:7123/api/SubscriberTelegramBots/GetSubscriber?id={chatID}");

            if (check == 1)
            {
                if (message.Sticker == message.Sticker)
                {
                    await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Ало");
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
        catch
        {
            return;
        }*/
        

        try
        {
            
            if (!message.Text.IsNullOrEmpty())
            {
                var findChatID = await httpClient.GetFromJsonAsync<SubscriberTelegramBot>($"https://localhost:7123/api/SubscriberTelegramBots/GetSubscriberCheckNull?id={message?.Chat.Id}");
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
            
                
                if (ListOffenders != null)
                {
                    foreach (var offender in ListOffenders)
                    {
                        List<SubscriberTelegramBot>? Subscribers = await httpClient.GetFromJsonAsync<List<SubscriberTelegramBot>>
                        ("https://localhost:7123/api/SubscriberTelegramBots/GetSubscriberTelegramBots");

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
                /*var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
                while (await timer.WaitForNextTickAsync())
                {
                    Update(botClient, update, token);
                }*/
                
            

        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
            Console.ReadLine();
        }
    }

    async static Task CheckOffender(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        BotClient = botClient;
        Update = update;
        Token = token;
        try
        {
            ListOffenders = await httpClient.GetFromJsonAsync<List<Offender>>("https://localhost:7123/api/Offenders/GetOffenders");
            foreach(var offender in ListOffenders)
            {
                if(offender.SendOrNot == 0)
                {
                    await UpdateM(botClient, update, token);
                }
            }
        }
        catch(Exception ex)
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
