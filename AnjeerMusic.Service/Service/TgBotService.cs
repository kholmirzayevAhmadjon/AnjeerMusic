using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using AnjeerMusic.Models.UserModels;

namespace AnjeerMusic.Service.Service;

public class TgBotService
{
    private TelegramBotClient client;
    private UserService userService;
    public TgBotService(TelegramBotClient client,UserService userService)
    {
        this.client = client;
        this.userService = userService;
    }

    public async Task Run()
    {
        client.StartReceiving(Update, Error);
        Console.ReadLine();
    }

    public async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
    {
        var message = update.Message;
        if (message?.Text == "/start")
        {
            await UserCreateAsync(message);
             
        }
    }

    public async Task UserCreateAsync(Message message)
    {
        UserCreationModel user = new UserCreationModel()
        {
            ChatId = message.Chat.Id,
            FirstName = message.Chat.FirstName,
            LastName = message.Chat.LastName
        };

        await client.SendTextMessageAsync(message.Chat.Id, $"Hi {message?.Chat?.FirstName?.ToString()}, " +
               $"Welcome to our Free Cloud Memeory");

        await userService.CreateAsync(user);
    }

    public async Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        Console.WriteLine(exception.Message);
    }
}
