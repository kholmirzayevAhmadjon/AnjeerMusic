using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using AnjeerMusic.Models.UserModels;
using AnjeerMusic.Domain.Configurations;
using AnjeerMusic.Models.MusicModels;

namespace AnjeerMusic.Service.Service;

public class TgBotService
{
    private TelegramBotClient client;
    private UserService userService;
    private MusicService musicService;
    private UserViewModel existUser;
    public TgBotService(TelegramBotClient client,UserService userService,MusicService musicService)
    {
        this.client = client;
        this.userService = userService;
        this.musicService = musicService;
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
            ShowMainMenu(message.Chat.Id);
        }
        else if(message?.Chat.Id == 1653251416 && message.Type == MessageType.Audio)
        {
           var filePath = await DownLoadMusicAsync(message);

           await MusicCreateAsync(message,filePath);
        }
        else if (message?.Text != null && message.Text.StartsWith("/"))
        {
            string command = message.Text.ToLower();

            switch (command)
            {
                case "/settings":
                    await client.SendTextMessageAsync(message.Chat.Id, "Settings menu is not implemented yet.");
                    break;
                case "/mymusic":
                    await client.SendTextMessageAsync(message.Chat.Id, "My Music menu is not implemented yet.");
                    break;
                case "/search":
                   // await client.SendTextMessageAsync(message.Chat.Id, "Search menu is not implemented yet.");
                    break;
                default:
                    await client.SendTextMessageAsync(message.Chat.Id, "Unknown command. Type /menu to see available commands.");
                    break;
            }
        }
        else if 
        (update.Type == UpdateType.CallbackQuery)
        {
            var callbackQuery = update.CallbackQuery;
            var chatId = callbackQuery.Message.Chat.Id;

            switch (callbackQuery.Data)
            {
                case "back_data":
                    // Handle "back" button click
                    break;
                case "exit_data":
                    // Remove the message when "❌" button is pressed
                    await client.DeleteMessageAsync(chatId, callbackQuery.Message.MessageId);
                    break;
                case "forward_data":
                    // Handle "forward" button click
                    break;
            }
        }
        else if(message.Type == MessageType.Text)
        {
            await SendMusicAsync(message.Chat.Id,message.Text);
        }
    }

    private void ShowMainMenu(long chatId)
    {
        var keyboard = new ReplyKeyboardMarkup(new[]
        {
             new[]
             {
                new KeyboardButton("/search"),
                new KeyboardButton("/mymusic")
             },
             new[]
             {
                new KeyboardButton("/settings")
             }
         });

        keyboard.ResizeKeyboard = true;
    }

    private InlineKeyboardMarkup ConditionInlineKeyboardMarkup()
    {
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {

                new[]
                {
                    InlineKeyboardButton.WithCallbackData("❤️", "heart_data"),

                    InlineKeyboardButton.WithCallbackData("❌", "exit_data"),
                }
            });

        return inlineKeyboard;
    }

    private InlineKeyboardMarkup DirectionInlineKeyboardMarkup()
    {
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("1️⃣", "number_1"),
                    InlineKeyboardButton.WithCallbackData("2️⃣", "number_2"),
                    InlineKeyboardButton.WithCallbackData("3️⃣", "number_3"),
                },
     
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️", "back_data"),

                    InlineKeyboardButton.WithCallbackData("❌", "exit_data"),

                    InlineKeyboardButton.WithCallbackData("➡️", "forward_data")
                }
            });

        return inlineKeyboard;
    }

    private async Task<string> DownLoadMusicAsync(Message message)
    {
        var audio = message.Audio;
        var file = await client.GetFileAsync(audio.FileId);
        string fileName = string.IsNullOrEmpty(audio.FileName) ? $"{audio.FileId}.mp3" : audio.FileName;
        string filePath = Path.Combine(Constants.fileDirectory, fileName);

        using (var saveStream = System.IO.File.OpenWrite(filePath))
        {
            await client.DownloadFileAsync(file.FilePath, saveStream);
        }
        return filePath;
    }

    private async Task MusicCreateAsync(Message message, string filePath)
    {
        byte[] fileBytes;
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fileBytes = new byte[fileStream.Length];
                fileStream.Read(fileBytes, 0, fileBytes.Length);
            }

        MusicCreationModel music = new()
        {
            Name = Path.GetFileName(filePath),
            MusicData = fileBytes
        };
        try
        {
             await musicService.CreateAsync(music);
             await client.SendTextMessageAsync(message.Chat.Id, $"Succesful added  😊", replyMarkup: ConditionInlineKeyboardMarkup());
        }
        catch (Exception ex)
        {
            await client.SendTextMessageAsync(message.Chat.Id, $"{ex.Message}");
        }

        System.IO.File.Delete(filePath);
    }

    private async Task SendMusicAsync(long chatId, string searchMusic)
    {
        var music = await musicService.SearchAsync(searchMusic);
        if(!music.Any()) 
        {
            await client.SendTextMessageAsync(chatId, $"Hech narsa topilmadi  😔", replyMarkup: ConditionInlineKeyboardMarkup());
        }
        else
        {
            using (var memoryStream = new MemoryStream(music.First().MusicData))
            {
                var audio = new InputFileStream(memoryStream, music.First().Name);
                await client.SendAudioAsync(chatId: chatId, audio: audio, replyMarkup: DirectionInlineKeyboardMarkup());
            }
        }
    }

    private async Task UserCreateAsync(Message message)
    {
        UserCreationModel user = new UserCreationModel()
        {
            ChatId = message.Chat.Id,
            FirstName = message.Chat.FirstName,
            LastName = message.Chat.LastName
        };

        await client.SendTextMessageAsync(message.Chat.Id, $"Hi {message?.Chat?.FirstName?.ToString()}, " +
               $"Welcome to our Free Music Bot");

        existUser = await userService.CreateAsync(user);
    }

    public async Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        Console.WriteLine(exception.Message);
    }
}
