using MongoDB.Driver;

using PartsHoleLib.Interfaces;

using PartsHoleRestLibrary.Enums;

namespace PartsHoleAPI.DBServices.Interfaces;

public interface IUserService
{
    IMongoCollection<IUserModel> UserCollection { get; init; }

    Task<bool> AddToDatabaseAsync(IUserModel data);
    Task<bool> DeleteFromDatabaseAsync(string id);
    Task<IUserModel?> GetFromDatabaseAsync(string id);
    Task<IUserData?> GetUserDataFromDatabaseAsync(IUserModel user);
    Task<bool> UpdateDatabaseAsync(string id, IUserModel data);
    Task<bool> RemoveModelFromUserAsync(string userId, string modelId, ModelIDSelector selector);
    Task<bool> AppendModelToUserAsync(string userId, string modelId, ModelIDSelector selector);
}