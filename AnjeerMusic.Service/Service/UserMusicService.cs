using AnjeerMusic.Data.Repositories;
using AnjeerMusic.Domain.UserMusics;
using AnjeerMusic.Models.UserModels;
using AnjeerMusic.Models.UserMusicModels;
using AnjeerMusic.Service.Extensions;
using AnjeerMusic.Service.Interface;

namespace AnjeerMusic.Service.Service;

public class UserMusicService : IUserMusicService
{
    private readonly Repository<UserMusic> repository;
    public UserMusicService(Repository<UserMusic> repository)
    {
        this.repository = repository;
    }

    public async Task<UserMusicViewModels> CreateAsync(UserCreationModel model)
    {
        var existUserMusic = await repository.InsertAsync(model.MapTo<UserMusic>());
        await repository.SaveAsync();
        return existUserMusic.MapTo<UserMusicViewModels>();
    }

    public async Task<IEnumerable<UserMusicViewModels>> GetAllAsync(long userId)
    {
        return repository.SelectAllAsQueryable()
            .Where(usermusic => usermusic.UserId == userId)
            .MapTo<UserMusicViewModels>().ToList();
    }

    public Task<UserMusicViewModels> UpdateAsync(long id, UserMusicViewModels model)
    {
        throw new NotImplementedException();
    }
}
