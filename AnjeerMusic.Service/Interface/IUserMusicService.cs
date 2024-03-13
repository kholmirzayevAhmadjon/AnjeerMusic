using AnjeerMusic.Models.UserMusicModels;

namespace AnjeerMusic.Service.Interface;

public interface IUserMusicService
{
    Task<UserMusicViewModels> CreateAsync(UserMusicCreationModels model);

    Task<IEnumerable<UserMusicViewModels>> GetAllAsync(long userId);
}