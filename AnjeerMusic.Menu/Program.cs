
using AnjeerMusic.Data.DbContexts;
using AnjeerMusic.Data.Repositories;
using AnjeerMusic.Domain.Configurations;
using AnjeerMusic.Domain.Users;
using AnjeerMusic.Service.Service;
using Telegram.Bot;


AppDbContext context = new AppDbContext();
Repository<User> repository = new Repository<User>(context);
UserService userService = new UserService(repository);
TelegramBotClient client = new TelegramBotClient(Constants.token);
TgBotService tgBotService = new TgBotService(client,userService);

await tgBotService.Run();