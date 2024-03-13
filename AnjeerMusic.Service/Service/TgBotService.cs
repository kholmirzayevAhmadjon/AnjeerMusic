using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using AnjeerMusic.Models.UserModels;
using AnjeerMusic.Domain.Configurations;
using AnjeerMusic.Models.MusicModels;
using AnjeerMusic.Domain.Musics;
using AnjeerMusic.Models.UserMusicModels;

namespace AnjeerMusic.Service.Service;

public class TgBotService
{
    private TelegramBotClient client;
    private UserService userService;
    private MusicService musicService;
    private UserViewModel existUser;
    private UserMusicService userMusicService;
    private  int currentPage = 1;
    private  int itemsPerPage = 3;
    private List<MusicViewModel> musicals;
    public TgBotService(TelegramBotClient client,UserService userService,MusicService musicService,UserMusicService userMusicService)
    {
        this.client = client;
        this.userService = userService;
        this.musicService = musicService;
        this.userMusicService = userMusicService;
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
            await ShowMainMenu(message.Chat.Id);
        }
        else if(message?.Chat.Id == 1966755645 && message.Type == MessageType.Audio)
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
                    await client.SendTextMessageAsync
                        (message.Chat.Id, "Settings ⚙️", replyMarkup: new InlineKeyboardMarkup(SettingsInlineKeyboardMarkup()));
                    break;
                case "/mymusic":
                    await MyMusicAsync(message.Chat.Id);
                    break;
                case "/search":
                    await SearchAsync(message.Chat.Id);
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
                case "exit_data":
                    await client.DeleteMessageAsync(chatId, callbackQuery.Message.MessageId);
                    break;
                case "heart_data":
                    //await client.DeleteOrCreateMusicAsync(chatId, callbackQuery.Message.MessageId);
                    break;
                case "mymusic_data":
                    await MyMusicAsync(chatId);
                    break;
                case "search_data":
                    await SearchAsync (chatId);
                    break;
                case "back_data":
                    currentPage--;
                    await SendPaginatedMessageAsync(chatId);
                    break;
                case "forward_data":
                    currentPage++;
                    await SendPaginatedMessageAsync(chatId);
                    break;
                case string s when s.StartsWith("audio_"):
                    var index = int.Parse(s.Split('_')[1]) - 1;
                    await SendMusicAsync(chatId,index);
                    break;
            }
        }
        else if (message.Type == MessageType.Text)
        {
            await SearchMusicAsync(message.Chat.Id, message.Text);
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

    private async Task SearchMusicAsync(long chatId, string searchMusic)
    {
        musicals = (await musicService.SearchAsync(searchMusic)).ToList();

        if (!musicals.Any())
        {
            await client.SendTextMessageAsync(chatId, $"Hech narsa topilmadi  😔", replyMarkup: new InlineKeyboardMarkup(GeneratePaginationKeyboard()));
        }
        else
        {
            await SendPaginatedMessageAsync(chatId);
        }
    }
   
    private async Task SendPaginatedMessageAsync(long chatId)
    {
        var musicalsCount = Convert.ToDouble(musicals.Count);
        var totalPages = (int)Math.Ceiling(musicalsCount / itemsPerPage);
        if (currentPage < 1)
            currentPage = 1;
        else if (currentPage > totalPages)
            currentPage = totalPages;
        var startIndex = (currentPage - 1) * itemsPerPage;
        var endIndex = Math.Min(startIndex + itemsPerPage - 1, musicalsCount - 1);

        var messageText = $"Page {currentPage} of {totalPages}:\n";
        var keyboard = new List<List<InlineKeyboardButton>>();

        int j = 1;
        var rowButtons = new List<InlineKeyboardButton>();
        for (int i = startIndex; i <= endIndex; i++)
        {
            messageText += $" {j}. {musicals[i].Name}\n";
            rowButtons.Add(InlineKeyboardButton.WithCallbackData($"{j}", $"audio_{i + 1}"));
            j++;
            if (j == itemsPerPage + 1)
                j = 1;
        }
        keyboard.Add(rowButtons);

        var paginationKeyboard = GeneratePaginationKeyboard();
        keyboard.Add(paginationKeyboard);
        await client.SendTextMessageAsync(chatId, messageText, replyMarkup: new InlineKeyboardMarkup(keyboard));
    }

    private async Task SendMusicAsync(long chatId, int index)
    {
        using (var memoryStream = new MemoryStream(musicals[index].MusicData))
        {
            var audio = new InputFileStream(memoryStream, musicals[index].Name);
            await client.SendAudioAsync(chatId: chatId, audio: audio, replyMarkup: ConditionInlineKeyboardMarkup());
        }

        UserMusicCreationModels userMusic = new()
        {
            UserId = existUser.Id,
            MusicId = musicals[index].Id
        };
        await userMusicService.CreateAsync(userMusic);
    }

    private async Task ShowMainMenu(long chatId)
    {
        var MenuKeyboard = new ReplyKeyboardMarkup(new[]
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

        MenuKeyboard.ResizeKeyboard = true;

        await client.SendTextMessageAsync(chatId, "Main Menu:", replyMarkup: MenuKeyboard);
    }

    private List<List<InlineKeyboardButton>> SettingsInlineKeyboardMarkup()
    {
        var SettingsKeyboard = new List<List<InlineKeyboardButton>>
    {
        new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData("MyMusic  🎵", "mymusic_data")
        },
        new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData("Search  🔍", "search_data")
        },
        new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData("❌", "exit_data")
        }
    };

        return SettingsKeyboard;
    }

    public async Task SearchAsync(long chatId)
    {
        await client.SendTextMessageAsync
                        (chatId, "Qo'shiqni topib berishim uchun, menga quyidagilardan birini yuboring\n" +
                        " Qo'shiq nomi\n Qo'shiq ijrochisi ismi");
    }
   
    private async Task MyMusicAsync(long chatId)
    {
        //musicals = (await userMusicService.GetAllAsync(existUser.Id)).ToList();

        if (!musicals.Any())
        {
            await client.SendTextMessageAsync(chatId, $"Hech narsa topilmadi  😔", replyMarkup: new InlineKeyboardMarkup(GeneratePaginationKeyboard()));
        }
        else
        {
            await SendPaginatedMessageAsync(chatId);
        }
    }

    private InlineKeyboardMarkup ConditionInlineKeyboardMarkup()
    {
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("❤️/💔", "heart_data"),

                    InlineKeyboardButton.WithCallbackData("❌", "exit_data"),
                }
            });

        return inlineKeyboard;
    }

    private List<InlineKeyboardButton> GeneratePaginationKeyboard()
    {
        var inlineKeyboard = new List<InlineKeyboardButton>() 
        {
            InlineKeyboardButton.WithCallbackData("⬅️", "back_data"),

            InlineKeyboardButton.WithCallbackData("❌", "exit_data"),

            InlineKeyboardButton.WithCallbackData("➡️", "forward_data")
        };
        return inlineKeyboard;
    }

    public async Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        Console.WriteLine(exception.Message);
    }
}