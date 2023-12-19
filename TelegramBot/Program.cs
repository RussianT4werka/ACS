using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Microsoft.IdentityModel.Tokens;
using LibraryBD.BD;
using System.Net.Http.Json;
using Update = Telegram.Bot.Types.Update;

class Program
{
    static ITelegramBotClient BotClient;
    static Update Update;
    static CancellationToken Token;

    private static ReceiverOptions _receiverOptions;

    private static TelegramBotClient client;

    static HttpClient httpClient = new HttpClient();
    private static List<SubscriberTelegramBot> ListSub { get; set; }
    private static List<Offender> ListOffendersNotSend { get; set; }

    static async Task Main(string[] args)
    {
        client = new TelegramBotClient("6732493440:AAGgyzhTGhjzc5YVO07sIaCNb6ksbMA4gcU");

        _receiverOptions = new ReceiverOptions // Настройки бота
        {
            AllowedUpdates = new[] // Тут указываем типы получаемых Update`ов, о них подробнее расказано тут https://core.telegram.org/bots/api#update
            {
                UpdateType.Message, // Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
            },
            ThrowPendingUpdates = true,
        };
        client.StartReceiving(UpdateM, Error, _receiverOptions);

        var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        while (await timer.WaitForNextTickAsync())
        {
            UpdateM(BotClient, Update, Token);
        }

        await Task.Delay(-1);
        Console.ReadLine();
    }
    async static Task UpdateM(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        ListSub = new();
        ListOffendersNotSend = new();
        SubscriberTelegramBot SubscriberTelegramBot;
        if (update != null)
        {
            var message = update.Message;
            var findChatID = await httpClient.GetFromJsonAsync<int>($"https://localhost:7123/api/SubscriberTelegramBots/GetSubscriberCheckNull?id={message?.Chat.Id}");
            if (!message.Text.IsNullOrEmpty() && message.Text.Contains("Хочу получать уведомления, 12345") && findChatID == 0)
            {
                SubscriberTelegramBot = new SubscriberTelegramBot()
                {
                    ChatId = (int)message.Chat.Id,
                    Username = message.Chat.Username,
                    Name = message.Chat.FirstName,
                    Surname = message.Chat.LastName
                };
                await httpClient.PostAsJsonAsync("https://localhost:7123/api/SubscriberTelegramBots/AddSubscriber", SubscriberTelegramBot);
            }
        }

        try
        {
            List<Offender>? ListOffenders = await httpClient.GetFromJsonAsync<List<Offender>>("https://localhost:7123/api/Offenders/GetOffenders");
            foreach (var offender in ListOffenders)
            {
                if (offender.SendOrNot == 0)
                {
                    ListOffendersNotSend.Add(offender);
                }
            }
            if(ListOffendersNotSend != null && ListOffendersNotSend.Count > 0)
            {
                List<SubscriberTelegramBot>? Subscribers = await httpClient.GetFromJsonAsync<List<SubscriberTelegramBot>>("https://localhost:7123/api/SubscriberTelegramBots/GetListSubscribers");
                foreach (var sub in Subscribers)
                {
                    if (sub.SubscribeOrNot == 1)
                    {
                        ListSub.Add(sub);
                    }
                }

                foreach (var offender in ListOffendersNotSend)
                {
                    foreach (var sub in ListSub)
                    {
                        await client.SendTextMessageAsync(
                        chatId: sub.ChatId,
                        text: $"Нарушитель:\n{offender.Name}\n{offender.Position}\n{offender.Time}");
                        Console.WriteLine($"Пользователь: {sub.Name} {sub.Surname} {sub.Username} получил сообщение:\n\"Нарушитель:\n{offender.Name}\n{offender.Position}\n{offender.Time}\"");
                    }
                    await httpClient.PostAsJsonAsync("https://localhost:7123/api/Offenders/SendOrNot", offender);
                }

                Console.WriteLine($"Отправлено {ListSub.Count()} подписчикам.");
                
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Ошибка : {ex}");
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
