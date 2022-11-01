using Microsoft.Extensions.Options;

using MongoDB.Driver;

using PartsHoleAPI.Utils;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.DBServices
{
   public class BinCollection : ICollectionService<BinModel>
   {
      #region Local Props
      public IMongoCollection<BinModel> Collection { get; init; }
      #endregion

      #region Constructors
      public BinCollection(IOptions<DatabaseSettings> settings)
      {
         var client = new MongoClient(settings.Value.ConnectionString);
         Collection = client
            .GetDatabase(settings.Value.Name)
            .GetCollection<BinModel>(settings.Value.BinsCollection);
      }
      #endregion

      #region Methods
      public async Task<BinModel> GetFromDatabaseAsync(string id) => throw new NotImplementedException();
      public async Task<IEnumerable<BinModel>> GetFromDatabaseAsync(string[] ids) => throw new NotImplementedException();

      public async Task<IEnumerable<BinModel>> AddToDatabaseAsync(IEnumerable<BinModel> data) => throw new NotImplementedException();
      public async Task<BinModel> AddToDatabaseAsync(BinModel data) => throw new NotImplementedException();

      public async Task UpdateDatabaseAsync(IEnumerable<BinModel> data) => throw new NotImplementedException();
      public async Task UpdateDatabaseAsync(string id, BinModel data) => throw new NotImplementedException();

      public async Task<int> DeleteFromDatabaseAsync(string[] id) => throw new NotImplementedException();
      public async Task<bool> DeleteFromDatabaseAsync(string id) => throw new NotImplementedException();
      #endregion

      #region Full Props

      #endregion
   }
}
