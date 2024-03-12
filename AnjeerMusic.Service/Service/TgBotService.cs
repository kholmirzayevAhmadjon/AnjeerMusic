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
        }
        else if(message?.Chat.Id == 1653251416 && message.Type == MessageType.Audio)
        {
           var filePath = await DownLoadMusicAsync(message);

           await MusicCreateAsync(message,filePath);
        }
        else if(message.Type == MessageType.Text)
        {
            await SendMusicAsync(message.Chat.Id,message.Text);
        }
    }

    public async Task<string> DownLoadMusicAsync(Message message)
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

    public async Task MusicCreateAsync(Message message, string filePath)
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
             await client.SendTextMessageAsync(message.Chat.Id, $"Succesful added");
        }
        catch (Exception ex)
        {
            await client.SendTextMessageAsync(message.Chat.Id, $"{ex.Message}");
        }

        System.IO.File.Delete(filePath);
    }

    public async Task SendMusicAsync(long chatId, string searchMusic)
    {
        var music = await musicService.SearchAsync(searchMusic);
        using (var memoryStream = new MemoryStream(music.First().MusicData))
        { 
            var audio = new InputFileStream(memoryStream, music.First().Name); 
            await client.SendAudioAsync(chatId:chatId,audio:audio);
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

        existUser = await userService.CreateAsync(user);
    }

    public async Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        Console.WriteLine(exception.Message);
    }
}
