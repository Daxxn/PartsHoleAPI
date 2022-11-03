using MongoDB.Driver;

namespace PartsHoleAPI.DBServices
{
   public interface IModelService<T>
   {
      IMongoCollection<T> Collection { get; init; }

      Task<T?> GetFromDatabaseAsync(string id);
      Task<bool> AddToDatabaseAsync(T data);
      Task<bool> UpdateDatabaseAsync(string id, T data);
      Task<bool> DeleteFromDatabaseAsync(string id);
   }
}
