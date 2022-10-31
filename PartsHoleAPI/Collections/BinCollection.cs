using Microsoft.Extensions.Options;

using MongoDB.Driver;

using PartsHoleAPI.Models;
using PartsHoleAPI.Models.Interfaces;

namespace PartsHoleAPI.Collections
{
   public class BinCollection : ICollection<IBinModel>
   {
      #region Local Props
      public IMongoClient Client { get; init; }
      private readonly IMongoDatabase _database;
      #endregion

      #region Constructors
      public BinCollection(IOptions<DatabaseSettings> settings)
      {
         Client = new MongoClient(settings.Value.DatabaseName);
         _database = Client.GetDatabase(settings.Value.BinCollection);
      }


      public IEnumerable<IBinModel> AddToDatabase(IEnumerable<IBinModel> data) => throw new NotImplementedException();
      public IBinModel AddToDatabase(IBinModel data) => throw new NotImplementedException();
      public int DeleteFromDatabase(string[] id) => throw new NotImplementedException();
      public bool DeleteFromDatabase(string id) => throw new NotImplementedException();
      public IEnumerable<IBinModel> GetFromDatabase(string[] ids) => throw new NotImplementedException();
      public IBinModel GetFromDatabase(string id) => throw new NotImplementedException();
      public void UpdateDatabase(IEnumerable<IBinModel> data) => throw new NotImplementedException();
      public void UpdateDatabase(IBinModel data) => throw new NotImplementedException();
      #endregion

      #region Methods

      #endregion

      #region Full Props

      #endregion
   }
}
