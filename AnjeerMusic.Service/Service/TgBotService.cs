using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AnjeerMusic.Service.Service;

public class TgBotService
{
    private TelegramBotClient client;
    public TgBotService(TelegramBotClient client)
    {
        this.client = client;
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
           
    }

    public async Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        Console.WriteLine(exception.Message);
    }
}
