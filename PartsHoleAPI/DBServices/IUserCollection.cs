using MongoDB.Driver;

using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.DBServices
{
   public interface IUserCollection
   {
      IMongoCollection<IUserModel> Collection { get; init; }

      Task<bool> AddToDatabaseAsync(IUserModel data);
      Task<bool> DeleteFromDatabaseAsync(string id);
      Task<IUserModel?> GetFromDatabaseAsync(string id);
      Task<IUserData?> GetUserDataFromDatabaseAsync(IUserModel user);
      Task<bool> UpdateDatabaseAsync(string id, IUserModel data);
   }
}