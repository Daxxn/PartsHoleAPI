using MongoDB.Driver;

namespace PartsHoleAPI.Collections
{
   public interface ICollectionService<T> : IModelService<T>
   {
      Task<IEnumerable<T>?> GetFromDatabase(string[] ids);

      Task<IEnumerable<T>?> AddToDatabase(IEnumerable<T> data);

      Task UpdateDatabase(IEnumerable<T> data);

      Task<int> DeleteFromDatabase(string[] id);
   }
}
