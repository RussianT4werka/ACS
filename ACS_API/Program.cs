using ACS_API;
using LibraryBD.BD;
using Telegram.Bot.Polling;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Json;
using Update = Telegram.Bot.Types.Update;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Telegram.Bot.Types.Enums;

namespace ACS_API
{
    class Program
    {
        static ITelegramBotClient BotClient;
        static Update Update;
        static CancellationToken Token;
        private static ReceiverOptions _receiverOptions;
        private static TelegramBotClient client;
        private static Log log;
        static HttpClient httpClient = new HttpClient();
        private static List<SubscriberTelegramBot> ListSub { get; set; } = new();
        private static List<Offender> ListOffendersNotSend { get; set; }
        public static void Main(string[] args)
        {
            client = new TelegramBotClient("6732493440:AAGgyzhTGhjzc5YVO07sIaCNb6ksbMA4gcU");

            _receiverOptions = new ReceiverOptions // Настройки бота
            {
                AllowedUpdates = new[] // Тут указываю тип получаемых Update`ов http://core.telegram.org/bots/api#update
                {
                UpdateType.Message, // Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
            },
                ThrowPendingUpdates = true,
            };
            client.StartReceiving(UpdateM, Error, _receiverOptions);

            Thread timer = new Thread(Timer);
            timer.Start();



            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<AcsContext>();
            builder.Services.AddSignalR();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapHub<NotifitionsHub>("notifications");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        static void Timer()
        {
            while (true)
            {
                var createOffender = httpClient.PostAsJsonAsync("http://10.10.1.102:7123/api/Offenders/CreateOffender","");
                var createCycle = httpClient.PostAsJsonAsync("http://10.10.1.102:7123/api/Cycles/CreateCycle", "");
                UpdateM(BotClient, Update, Token);
                Thread.Sleep(1000);
            }
        }

        async static Task UpdateM(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            ListOffendersNotSend = new();
            SubscriberTelegramBot SubscriberTelegramBot;
            try
            {
                if (update != null && update.Message.Text == "/start")
                {
                    // Создание клаиватуры
                    var inlineKeyboard = new InlineKeyboardMarkup(
                    new List<InlineKeyboardButton[]>()
                    {
                    new InlineKeyboardButton[] // Создание массива кнопок
                    {
                        InlineKeyboardButton.WithUrl("Сайт компании", "http://safecity.pro/"),
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
                        // тогда клавиатура растягивается
                        ResizeKeyboard = true,
                    };

                    await botClient.SendTextMessageAsync(
                        update.Message.Chat.Id, "Пиши \"Хочу получать уведомления\" если хочешь получать уведмления)", replyMarkup: replyKeyboard); // опять передаем клавиатуру в параметр replyMarkup

                    return;
                }

                if (update != null)
                {
                    var message = update.Message;
                    var findChatID = await httpClient.GetFromJsonAsync<int>($"http://10.10.1.102:7123/api/SubscriberTelegramBots/GetSubscriberCheckNull?id={message?.Chat.Id}");
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
                        await httpClient.PostAsJsonAsync("http://10.10.1.102:7123/api/SubscriberTelegramBots/AddSubscriber", SubscriberTelegramBot);
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, $"{update.Message.Chat.Username}, подписал вас на уведомления. Нужно только согласовать с администратором");
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, $"{update.Message.Chat.Username}, вы уже подписаны на уведомления");
                    }
                }


                List<Offender>? ListOffenders = await httpClient.GetFromJsonAsync<List<Offender>>("http://10.10.1.102:7123/api/Offenders/GetOffenders");
                foreach (var offender in ListOffenders)
                {
                    if (offender.SendOrNot == 0)
                    {
                        ListOffendersNotSend.Add(offender);
                    }
                }
                if (ListOffendersNotSend != null && ListOffendersNotSend.Count > 0)
                {
                    
                    List<SubscriberTelegramBot>? Subscribers = await httpClient.GetFromJsonAsync<List<SubscriberTelegramBot>>("http://10.10.1.102:7123/api/SubscriberTelegramBots/GetListSubscribers");

                    foreach (var sub in Subscribers)
                    {
                        if (sub.SubscribeOrNot == 1)
                        {
                            ListSub.Add(sub);
                        }
                    }

                    foreach (var offender in ListOffendersNotSend)
                    {
                        if(ListSub == null || ListSub.Count() == 0)
                        {
                            await httpClient.PostAsJsonAsync("http://10.10.1.102:7123/api/Offenders/SendOrNot", offender); 
                        }
                        else
                        {
                            foreach (var sub in ListSub)
                            {
                                if(offender.Position == "Водитель АБС")
                                {
                                    await client.SendTextMessageAsync(
                                chatId: sub.ChatId,
                                text: $"Нарушитель:\n{offender.Name}\n{offender.Position}\nПричина: нарушение регламента рабочего времени\n{offender.Time}");
                                    log = new Log() { Title = $"Пользователь: {sub.Name} {sub.Surname} {sub.Username} получил сообщение:\n\"Нарушитель:\n{offender.Name}\n{offender.Position}\n{offender.Time}\"", DateTime = DateTime.Now };
                                    await httpClient.PostAsJsonAsync("http://10.10.1.102:7123/api/Logs/WriteLog", log);
                                    await httpClient.PostAsJsonAsync("http://10.10.1.102:7123/api/Offenders/SendOrNot", offender);
                                    Console.WriteLine(log.Title);
                                }
                                else
                                {
                                    await client.SendTextMessageAsync(
                                chatId: sub.ChatId,
                                text: $"Нарушитель:\n{offender.Name}\n{offender.Position}\nПричина: попытка несанкционированного доступа\n{offender.Time}");
                                    log = new Log() { Title = $"Пользователь: {sub.Name} {sub.Surname} {sub.Username} получил сообщение:\n\"Нарушитель:\n{offender.Name}\n{offender.Position}\n{offender.Time}\"", DateTime = DateTime.Now };
                                    await httpClient.PostAsJsonAsync("http://10.10.1.102:7123/api/Logs/WriteLog", log);
                                    await httpClient.PostAsJsonAsync("http://10.10.1.102:7123/api/Offenders/SendOrNot", offender);
                                    Console.WriteLine(log.Title);
                                }
                                
                            }
                            log = new Log() { Title = $"Уведомление отправлено {ListSub.Count()} подписчикам.", DateTime = DateTime.Now };
                            ListSub.Clear();
                            await httpClient.PostAsJsonAsync("http://10.10.1.102:7123/api/Logs/WriteLog", log);
                            Console.WriteLine(log.Title);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var exeption = $"Ошибка : {ex}";
                log = new Log() { Title = exeption, DateTime = DateTime.Now };
                httpClient.PostAsJsonAsync("http://10.10.1.102:7123/api/Logs/WriteLog", log);
                Console.WriteLine(exeption);
                Console.ReadLine();
            }
        }

        private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token) //Метод обработки ошибок Teleram
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            log = new Log() { Title = ErrorMessage, DateTime = DateTime.Now };
            httpClient.PostAsJsonAsync("http://10.10.1.102:7123/api/Logs/WriteLog", log);
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

    }
}


    

