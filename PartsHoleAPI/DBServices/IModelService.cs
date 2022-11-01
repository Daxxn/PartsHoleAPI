using MongoDB.Driver;

namespace PartsHoleAPI.Collections
{
   public interface IModelService<T>
   {
      IMongoCollection<T> Collection { get; init; }
      Task<T?> GetFromDatabaseAsync(string id);
      Task<T?> AddToDatabaseAsync(T data);
      Task UpdateDatabase(T data);
      Task<bool> DeleteFromDatabase(string id);
   }
}
