using AnjeerMusic.Data.Repositories;
using AnjeerMusic.Domain.Musics;
using AnjeerMusic.Models.MusicModels;
using AnjeerMusic.Service.Extensions;
using AnjeerMusic.Service.Interface;

namespace AnjeerMusic.Service.Service;

public class MusicService : IMusicService
{
    private readonly IRepository<Music> repository;
    public async Task<MusicViewModel> CreateAsync(MusicCreationModel music)
    {
        var existingMusic = repository
            .SelectAllAsQueryable()
            .FirstOrDefault(m => m.Name == music.Name);

        if (existingMusic != null)
            throw new Exception("Music already exists");
        
        var createdMusic = await repository.InsertAsync(music.MapTo<Music>());
        await repository.SaveAsync();
        
        return createdMusic.MapTo<MusicViewModel>();
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var existingMusic = await repository.SelectByIdAsync(id)
            ?? throw new Exception("Music not found");

        existingMusic.DeletedAt = DateTime.UtcNow;
        existingMusic.IsDeleted = true;

        await repository.DeleteAsync(existingMusic);
        await repository.SaveAsync();

        return true;
    }

    public async Task<IEnumerable<MusicViewModel>> GetAllAsync()
    {
        return await Task.FromResult(repository.SelectAllAsEnumerable().MapTo<MusicViewModel>());
    }

    public async Task<IEnumerable<MusicViewModel>> SearchAsync(string searchString)
    {
        return await Task.FromResult(repository.SelectAllAsEnumerable()
            .Where(music => music.Name
                .Contains(searchString, StringComparison.OrdinalIgnoreCase))
            .MapTo<MusicViewModel>());
    }
}