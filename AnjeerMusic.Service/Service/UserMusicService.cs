using AnjeerMusic.Data.Repositories;
using AnjeerMusic.Domain.UserMusics;
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

    public async Task<UserMusicViewModels> CreateAsync(UserMusicCreationModels model)
    {
        var existUserMusic = await repository.InsertAsync(model.MapTo<UserMusic>());
        await repository.SaveAsync();
        return existUserMusic.MapTo<UserMusicViewModels>();
    }

    public async Task<IEnumerable<UserMusicViewModels>> GetAllAsync(long userId)
    {
        return await Task.FromResult(repository.SelectAllAsQueryable()
            .Where(usermusic => usermusic.UserId == userId)
            .MapTo<UserMusicViewModels>().ToList());
    }
}
