using MongoDB.Driver;

namespace PartsHoleAPI.Collections
{
   public interface IModelService <T>
   {
      IMongoClient Client { get; }
      T GetFromDatabase(string id);
      T AddToDatabase(T data);
      void UpdateDatabase(T data);
      bool DeleteFromDatabase(string id);
   }
}
