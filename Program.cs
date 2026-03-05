using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

internal class Program
{
    private static void Main(string[] args)//Точка входа в программу
    {
        Start();
        Console.ReadLine();
    }
    //Метод запуска обработки вашего бота
    private static async void Start()
    {
        //Вставляем наш токен в параметры класса TelegramBotClient
        var botClient = new TelegramBotClient(ConsoleTelegramBot.Properties.Resources.Token);

        using CancellationTokenSource cts = new();
        //Указываем параметры запросов
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };
        //Запускаем обработку бота
        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );
        //Выводим сообщение в консоль
        var me = await botClient.GetMeAsync();
        Console.WriteLine($"Start listening for @{me.Username ?? "bot"}");
        Console.ReadLine();

        cts.Cancel();
    }
    //Метод обработки Сообщений
    private static async Task HandleUpdateMessagesAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message || message.Text is not { } messageText)
            return;

        var chatId = message.Chat.Id;
        var userName = message.Chat.Username;

        //Вывод сообщения о сообщение и пользователе отправившего его 
        Console.WriteLine($"User Name - {userName},  ID - {chatId}\nMessage - {messageText}\n");

        //Обработка стартового сообщения
        if (messageText == "/start")
        {
            //Добавление кнопок в сообщении
            InlineKeyboardMarkup inlineKeyboard = new(new[]
            {
                new []//отдельная линия с кнопками
                {
                    InlineKeyboardButton.WithUrl(//добавление кнопки
                        text: "StackOverFlow",//название кнопки
                        url: "https://stackoverflow.com/"),//ссылка
                },
                new []
                {
                    InlineKeyboardButton.WithUrl(
                        text: "Мануал о создании",
                        url: "https://telegrambots.github.io/book/index.html"),

                    InlineKeyboardButton.WithUrl(
                        text: "О нас",
                        url: "https://1-mok.mskobr.ru/postuplenie-v-kolledzh/priemnaya-komissiya"),
                },
                // Новая линия кнопок (Помощник учёбы)
                new []
                {
                    InlineKeyboardButton.WithUrl(
                        text: "Документация C#",
                        url: "https://learn.microsoft.com/ru-ru/dotnet/csharp/"),
                    InlineKeyboardButton.WithUrl(
                        text: "GitHub",
                        url: "https://github.com"),
                },
            });
            //отпраляем собщение с кнопками
            await botClient.SendTextMessageAsync(
                 chatId: chatId,//Индентификатор чата в который отправляется сообщение
                 text: "Привет! Я Помощник учёбы — выберите ссылку ниже.",//Сообщение
                 replyMarkup: inlineKeyboard,//Добавленные к сообщению кнопки
                 cancellationToken: cancellationToken//Указываем методя для обработки исключений
                 );

            //Добавляем кнопкок-клавиатуры
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "StackOverFlow" },
                new KeyboardButton[] { "Мануал о создании", "О нас" },
                // Новая линия кнопок в клавиатуре
                new KeyboardButton[] { "Документация C#", "GitHub" },
            })
            {
                // Разрешаем автоматическое расширение размера кнопок
                ResizeKeyboard = true 
            };
            //отпраляем собщение с добавление клавиатуры
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Вот полезные кнопки для учёбы:",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
            return;
        }
        //Проверка отправляемых пользователем сообщений на соответствие
        switch (messageText)
        {
            //Должно совпадать с название текста которое требуется обработать
            case "StackOverFlow":
                //Отправляем сообщение
                await botClient.SendTextMessageAsync(
                  chatId: chatId,
                  text: "https://stackoverflow.com/",
                  cancellationToken: cancellationToken);
                break;//Окончание обработки текста

            case "Мануал о создании":
                await botClient.SendTextMessageAsync(
                  chatId: chatId,
                  text: "https://telegrambots.github.io/book/index.html",
                  cancellationToken: cancellationToken);
                break;
            case "О нас":
                await botClient.SendTextMessageAsync(
                  chatId: chatId,
                  text: "https://1-mok.mskobr.ru/postuplenie-v-kolledzh/priemnaya-komissiya",
                  cancellationToken: cancellationToken);
                break;

            case "Документация C#":
                await botClient.SendTextMessageAsync(
                  chatId: chatId,
                  text: "Документация по C#: https://learn.microsoft.com/ru-ru/dotnet/csharp/",
                  cancellationToken: cancellationToken);
                break;
            case "GitHub":
                await botClient.SendTextMessageAsync(
                  chatId: chatId,
                  text: "GitHub: https://github.com",
                  cancellationToken: cancellationToken);
                break;

            default://Обработка не распознанного сообщения
                await botClient.SendTextMessageAsync(
                  chatId: chatId,
                  text: "Сообщение не распознано. Нажмите одну из кнопок ниже.",
                  cancellationToken: cancellationToken);
                break;
        }

    }
    //Метод обработки действий пользователя
    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        //Проверка на наличие сообщение и текста в нём
        if (update.Message is not { } message)
        {
            return;
        }
        if (message.Text is not { } messageText)
        {
            return;
        }

        //Запуск Метода проверки сообщений
        await HandleUpdateMessagesAsync(botClient, update, cancellationToken);
    }

    //Метод для обратоки исключительных ситуаций
    private static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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