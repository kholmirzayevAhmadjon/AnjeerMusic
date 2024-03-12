using AnjeerMusic.Data.Repositories;
using AnjeerMusic.Domain.Users;
using AnjeerMusic.Models.UserModels;
using AnjeerMusic.Service.Extensions;
using AnjeerMusic.Service.Interface;

namespace AnjeerMusic.Service.Service;

public class UserService : IUserService
{
    private readonly Repository<User> repository;
    public UserService(Repository<User> repository)
    {
        this.repository = repository;
    }

    public async Task<UserViewModel> CreateAsync(UserCreationModel user)
    {
        var users = repository.SelectAllAsQueryable();
        var existUser = users.FirstOrDefault(u => u.ChatId == user.ChatId);
        if (existUser != null)
            return await UpdateAsync(existUser.Id, user.MapTo<UserUpdateModel>());

        var createUser = await repository.InsertAsync(user.MapTo<User>());
        return createUser.MapTo<UserViewModel>();
    }

    public Task<bool> DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<UserViewModel> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task<UserViewModel> UpdateAsync(long id, UserUpdateModel user)
    {
        throw new NotImplementedException();
    }
}
