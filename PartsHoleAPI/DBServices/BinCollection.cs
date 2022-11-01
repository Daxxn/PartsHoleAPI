using Microsoft.Extensions.Options;

using MongoDB.Driver;

using PartsHoleAPI.Models;
using PartsHoleAPI.Models.Interfaces;

namespace PartsHoleAPI.Collections
{
   public class BinCollection : ICollectionService<IBinModel>
   {
      #region Local Props
      public IMongoCollection<IBinModel> Collection { get; init; }
      #endregion

      #region Constructors
      public BinCollection(IOptions<DatabaseSettings> settings)
      {
         var client = new MongoClient(settings.Value.ConnectionString);
         Collection = client
            .GetDatabase(settings.Value.Name)
            .GetCollection<IBinModel>(settings.Value.BinsCollection);
      }
      #endregion

      #region Methods
      public async Task<IEnumerable<IBinModel>> AddToDatabase(IEnumerable<IBinModel> data) => throw new NotImplementedException();
      public async Task<IBinModel> AddToDatabaseAsync(IBinModel data) => throw new NotImplementedException();
      public async Task<int> DeleteFromDatabase(string[] id) => throw new NotImplementedException();
      public async Task<bool> DeleteFromDatabase(string id) => throw new NotImplementedException();
      public async Task<IEnumerable<IBinModel>> GetFromDatabase(string[] ids) => throw new NotImplementedException();
      public async Task<IBinModel> GetFromDatabaseAsync(string id) => throw new NotImplementedException();
      public async Task UpdateDatabase(IEnumerable<IBinModel> data) => throw new NotImplementedException();
      public async Task UpdateDatabase(IBinModel data) => throw new NotImplementedException();
      #endregion

      #region Full Props

      #endregion
   }
}
