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
        var existUser = repository.SelectAllAsQueryable().FirstOrDefault(u => u.ChatId == user.ChatId);
        if (existUser != null)
            return await UpdateAsync(existUser.Id, user.MapTo<UserUpdateModel>());

        var createUser = await repository.InsertAsync(user.MapTo<User>());
        await repository.SaveAsync();
        return createUser.MapTo<UserViewModel>();
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var existUser = await repository.SelectByIdAsync(id)
            ?? throw new Exception($"This user is not found With this id {id}");

        existUser.IsDeleted = true;
        existUser.DeletedAt = DateTime.UtcNow;

        await repository.DeleteAsync(existUser);
        await repository.SaveAsync();
        return true;
    }

    public async Task<UserViewModel> GetByIdAsync(long id)
    {
        var existUser = await repository.SelectByIdAsync(id)
            ?? throw new Exception($"This user is not found With this id {id}");

        return existUser.MapTo<UserViewModel>();
    }

    public async Task<UserViewModel> UpdateAsync(long id, UserUpdateModel user)
    {
        var existUser = await repository.SelectByIdAsync(id)
            ?? throw new Exception($"This user is not found With this id {id}");

        existUser.IsDeleted = false;
        existUser.UpdatedAt = DateTime.UtcNow;
        existUser.FirstName = user.FirstName;
        existUser.LastName = user.LastName;
        existUser.PhoneNumber = user.PhoneNumber;
        existUser.ChatId = user.ChatId;

        await repository.UpdateAsync(existUser);
        await repository.SaveAsync();
        return existUser.MapTo<UserViewModel>();
    }
}
