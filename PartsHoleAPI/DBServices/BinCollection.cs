using Microsoft.Extensions.Options;

using MongoDB.Driver;

using PartsHoleAPI.Utils;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.DBServices
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
      public async Task<IBinModel> GetFromDatabaseAsync(string id) => throw new NotImplementedException();
      public async Task<IEnumerable<IBinModel>> GetFromDatabaseAsync(string[] ids) => throw new NotImplementedException();

      public async Task<IEnumerable<IBinModel>> AddToDatabaseAsync(IEnumerable<IBinModel> data) => throw new NotImplementedException();
      public async Task<IBinModel> AddToDatabaseAsync(IBinModel data) => throw new NotImplementedException();

      public async Task UpdateDatabaseAsync(IEnumerable<IBinModel> data) => throw new NotImplementedException();
      public async Task UpdateDatabaseAsync(string id, IBinModel data) => throw new NotImplementedException();

      public async Task<int> DeleteFromDatabaseAsync(string[] id) => throw new NotImplementedException();
      public async Task<bool> DeleteFromDatabaseAsync(string id) => throw new NotImplementedException();
      #endregion

      #region Full Props

      #endregion
   }
}
