using MongoDB.Driver;

namespace PartsHoleAPI.DBServices
{
   public interface IModelService<T>
   {
      IMongoCollection<T> Collection { get; init; }

      Task<T?> GetFromDatabaseAsync(string id);
      Task<T?> AddToDatabaseAsync(T data);
      Task UpdateDatabaseAsync(string id, T data);
      Task<bool> DeleteFromDatabaseAsync(string id);
   }
}
