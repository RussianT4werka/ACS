using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Microsoft.IdentityModel.Tokens;
using LibraryBD.BD;
using System.Net.Http.Json;
using Update = Telegram.Bot.Types.Update;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System;

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
        try
        {
            if (update != null && update.Message.Text == "/start")
            {
                // Тут создаем нашу клавиатуру
                var inlineKeyboard = new InlineKeyboardMarkup(
                new List<InlineKeyboardButton[]>()
                {
                    new InlineKeyboardButton[] // тут создаем массив кнопок
                    {
                        InlineKeyboardButton.WithUrl("Сайт компании", "https://safecity.pro/"),
                    }
                });
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, $"Привет {update.Message.Chat.Username}!", replyMarkup: inlineKeyboard); // Все клавиатуры передаются в параметр replyMarkup

                var replyKeyboard = new ReplyKeyboardMarkup(
                                    new List<KeyboardButton[]>()
                                    {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Хочу получать уведомления!"),
                                            //new KeyboardButton("Пока!"),
                                        }
                                    })
                {
                    // автоматическое изменение размера клавиатуры, если не стоит true,
                    // тогда клавиатура растягивается чуть ли не до луны,
                    // проверить можете сами
                    ResizeKeyboard = true,
                };

                await botClient.SendTextMessageAsync(
                    update.Message.Chat.Id, "Пиши \"Хочу получать уведомления\" если хочешь получать уведмления)", replyMarkup: replyKeyboard); // опять передаем клавиатуру в параметр replyMarkup

                return;
            }

        
            if (update != null)
            {
                var message = update.Message;
                var findChatID = await httpClient.GetFromJsonAsync<int>($"https://localhost:7123/api/SubscriberTelegramBots/GetSubscriberCheckNull?id={message?.Chat.Id}");
                if (!message.Text.IsNullOrEmpty() && message.Text.Contains("Хочу получать уведомления") && findChatID == 0 || findChatID == 0)
                {
                    string stringChatId = Convert.ToString(message.Chat.Id);
                    SubscriberTelegramBot = new SubscriberTelegramBot()
                    {
                        ChatId = stringChatId,
                        Username = message.Chat.Username,
                        Name = message.Chat.FirstName,
                        Surname = message.Chat.LastName
                    };
                    await httpClient.PostAsJsonAsync("https://localhost:7123/api/SubscriberTelegramBots/AddSubscriber", SubscriberTelegramBot);
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, $"{update.Message.Chat.Username}, подписал вас на уведомления. Нужно только согласовать с администратором");
                }
                else
                {
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, $"{update.Message.Chat.Username}, вы уже подписаны на уведомления");
                }
            }

        
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
                        Console.WriteLine($"{DateTime.Now} - Пользователь: {sub.Name} {sub.Surname} {sub.Username} получил сообщение:\n\"Нарушитель:\n{offender.Name}\n{offender.Position}\n{offender.Time}\"");
                    }
                    await httpClient.PostAsJsonAsync("https://localhost:7123/api/Offenders/SendOrNot", offender);
                }

                Console.WriteLine($"{DateTime.Now} - Отправлено {ListSub.Count()} подписчикам.");
                
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine($"{DateTime.Now} - Ошибка : {ex}");
            Console.ReadLine();
        }
    }

    private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"{DateTime.Now} - Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}
