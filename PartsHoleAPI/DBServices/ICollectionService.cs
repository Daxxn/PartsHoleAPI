using MongoDB.Driver;

namespace PartsHoleAPI.DBServices
{
   public interface ICollectionService<T> : IModelService<T>
   {
      Task<IEnumerable<T>?> GetFromDatabaseAsync(string[] ids);

      Task<IEnumerable<T>?> AddToDatabaseAsync(IEnumerable<T> data);

      Task UpdateDatabaseAsync(IEnumerable<T> data);

      Task<int> DeleteFromDatabaseAsync(string[] id);
   }
}
