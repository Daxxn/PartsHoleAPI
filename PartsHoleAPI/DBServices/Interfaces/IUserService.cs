using MongoDB.Driver;

using PartsHoleLib;
using PartsHoleLib.Enums;

namespace PartsHoleAPI.DBServices.Interfaces;

public interface IUserService
{
    IMongoCollection<UserModel> UserCollection { get; init; }

    Task<bool> AddToDatabaseAsync(UserModel data);
    Task<bool> DeleteFromDatabaseAsync(string id);
    Task<UserModel?> GetFromDatabaseAsync(string id);
    Task<UserData?> GetUserDataFromDatabaseAsync(UserModel user);
    Task<bool> UpdateDatabaseAsync(string id, UserModel data);
    Task<bool> RemoveModelFromUserAsync(string userId, string modelId, ModelIDSelector selector);
    Task<bool> AppendModelToUserAsync(string userId, string modelId, ModelIDSelector selector);
}