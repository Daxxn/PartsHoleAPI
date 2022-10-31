using MongoDB.Driver;

using PartsHoleAPI.Models;
using PartsHoleAPI.Models.Interfaces;

namespace PartsHoleAPI.Collections
{
   public class PartsCollection : ICollection<IPartModel>
   {
      #region Local Props
      public IMongoClient Client { get; init; }
      private IMongoDatabase _database;
      #endregion

      #region Constructors
      public PartsCollection(IMongoClient client)
      {
         Client = client;
         _database = Client.GetDatabase("Inventory");
      }
      #endregion

      #region Methods
      public IEnumerable<IPartModel> GetFromDatabase(string[] ids)
      {
         var filter = Builders<IPartModel>.Filter.Where(x => ids.Contains(x.Id));
         return _database.GetCollection<IPartModel>("Parts").Find(filter).ToList();
      }
      public void UpdateDatabase(IPartModel data)
      {
         var filter = Builders<IPartModel>.Filter.Where(x => data.Id == x.Id);

         _database.GetCollection<IPartModel>("Parts").ReplaceOne(filter, data);
      }
      public void UpdateDatabase(IEnumerable<IPartModel> data)
      {

      }

      public IPartModel GetFromDatabase(string id) => throw new NotImplementedException();
      public IPartModel AddToDatabase(IPartModel data) => throw new NotImplementedException();
      public IEnumerable<IPartModel> AddToDatabase(IEnumerable<IPartModel> data) => throw new NotImplementedException();
      public bool DeleteFromDatabase(string id) => throw new NotImplementedException();
      public int DeleteFromDatabase(string[] id) => throw new NotImplementedException();
      #endregion

      #region Full Props

      #endregion
   }
}
