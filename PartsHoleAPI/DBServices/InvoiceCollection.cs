using Microsoft.Extensions.Options;

using MongoDB.Driver;

using PartsHoleAPI.Utils;

using PartsHoleLib;

namespace PartsHoleAPI.DBServices
{
   public class InvoiceCollection : ICollectionService<InvoiceModel>
   {
      #region Local Props
      public IMongoCollection<InvoiceModel> Collection { get; init; }
      #endregion

      #region Constructors
      public InvoiceCollection(IOptions<DatabaseSettings> settings)
      {
         var client = new MongoClient(settings.Value.ConnectionString);
         Collection = client
            .GetDatabase(settings.Value.Name)
            .GetCollection<InvoiceModel>(settings.Value.InvoiceCollection);
      }
      #endregion

      #region Methods
      public Task<IEnumerable<InvoiceModel>?> GetFromDatabaseAsync(string[] ids) => throw new NotImplementedException();
      public Task<InvoiceModel?> GetFromDatabaseAsync(string id) => throw new NotImplementedException();

      public Task<IEnumerable<InvoiceModel>?> AddToDatabaseAsync(IEnumerable<InvoiceModel> data) => throw new NotImplementedException();
      public Task<InvoiceModel?> AddToDatabaseAsync(InvoiceModel data) => throw new NotImplementedException();

      public Task UpdateDatabaseAsync(IEnumerable<InvoiceModel> data) => throw new NotImplementedException();
      public Task UpdateDatabaseAsync(string id, InvoiceModel data) => throw new NotImplementedException();

      public Task<int> DeleteFromDatabaseAsync(string[] id) => throw new NotImplementedException();
      public Task<bool> DeleteFromDatabaseAsync(string id) => throw new NotImplementedException();
      #endregion

      #region Full Props

      #endregion
   }
}
