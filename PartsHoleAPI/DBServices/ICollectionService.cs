using MongoDB.Driver;

namespace PartsHoleAPI.DBServices
{
   public interface ICollectionService<T> : IModelService<T>
   {
      Task<IEnumerable<T>?> GetFromDatabaseAsync(string[] ids);

      Task<IEnumerable<bool>?> AddToDatabaseAsync(IEnumerable<T> data);

      Task<IEnumerable<bool>?> UpdateDatabaseAsync(IEnumerable<T> data);

      Task<int> DeleteFromDatabaseAsync(string[] id);
   }
}
