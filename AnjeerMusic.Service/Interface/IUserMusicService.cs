using AnjeerMusic.Models.UserModels;
using AnjeerMusic.Models.UserMusicModels;

namespace AnjeerMusic.Service.Interface;

public interface IUserMusicService
{
    Task<UserMusicViewModels> CreateAsync(UserCreationModel model);

    Task<UserMusicViewModels> UpdateAsync(long id, UserMusicViewModels model);

    Task<IEnumerable<UserMusicViewModels>> GetAllAsync(long userId);
}

