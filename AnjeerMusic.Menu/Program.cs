
using AnjeerMusic.Data.DbContexts;
using AnjeerMusic.Data.Repositories;
using AnjeerMusic.Domain.Configurations;
using AnjeerMusic.Domain.Musics;
using AnjeerMusic.Domain.UserMusics;
using AnjeerMusic.Domain.Users;
using AnjeerMusic.Service.Service;
using Telegram.Bot;


AppDbContext context = new AppDbContext();
UserService userService = new UserService(new Repository<User>(context));
MusicService musicService = new MusicService(new Repository<Music>(context));
UserMusicService userMusicService = new UserMusicService(new Repository<UserMusic>(context));
TelegramBotClient client = new TelegramBotClient(Constants.token);
TgBotService tgBotService = new TgBotService(client,userService,musicService,userMusicService);

await tgBotService.Run();