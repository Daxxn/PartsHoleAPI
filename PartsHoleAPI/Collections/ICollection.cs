using MongoDB.Driver;

namespace PartsHoleAPI.Collections
{
   public interface ICollection<T> : IModelService<T>
   {
      IEnumerable<T> GetFromDatabase(string[] ids);

      IEnumerable<T> AddToDatabase(IEnumerable<T> data);

      void UpdateDatabase(IEnumerable<T> data);

      int DeleteFromDatabase(string[] id);
   }
}
