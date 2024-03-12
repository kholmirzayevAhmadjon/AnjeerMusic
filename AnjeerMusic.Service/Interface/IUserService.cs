using AnjeerMusic.Domain.Users;
using AnjeerMusic.Models.UserModels;

namespace AnjeerMusic.Service.Interface;

public interface IUserService
{
    Task<UserViewModel> CreateAsync(UserCreationModel user);

    Task<UserViewModel> UpdateAsync(long id, UserUpdateModel user);

    Task<bool> DeleteAsync(long id);

    Task<UserViewModel> GetByIdAsync(long id);
}

