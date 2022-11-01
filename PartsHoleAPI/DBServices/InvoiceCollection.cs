using Microsoft.Extensions.Options;

using MongoDB.Driver;

using PartsHoleAPI.Utils;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.DBServices
{
   public class InvoiceCollection : ICollectionService<IInvoiceModel>
   {
      #region Local Props
      public IMongoCollection<IInvoiceModel> Collection { get; init; }
      #endregion

      #region Constructors
      public InvoiceCollection(IOptions<DatabaseSettings> settings)
      {
         var client = new MongoClient(settings.Value.ConnectionString);
         Collection = client
            .GetDatabase(settings.Value.Name)
            .GetCollection<IInvoiceModel>(settings.Value.InvoiceCollection);
      }
      #endregion

      #region Methods
      public Task<IEnumerable<IInvoiceModel>?> GetFromDatabaseAsync(string[] ids) => throw new NotImplementedException();
      public Task<IInvoiceModel?> GetFromDatabaseAsync(string id) => throw new NotImplementedException();

      public Task<IEnumerable<IInvoiceModel>?> AddToDatabaseAsync(IEnumerable<IInvoiceModel> data) => throw new NotImplementedException();
      public Task<IInvoiceModel?> AddToDatabaseAsync(IInvoiceModel data) => throw new NotImplementedException();

      public Task UpdateDatabaseAsync(IEnumerable<IInvoiceModel> data) => throw new NotImplementedException();
      public Task UpdateDatabaseAsync(string id, IInvoiceModel data) => throw new NotImplementedException();

      public Task<int> DeleteFromDatabaseAsync(string[] id) => throw new NotImplementedException();
      public Task<bool> DeleteFromDatabaseAsync(string id) => throw new NotImplementedException();
      #endregion

      #region Full Props

      #endregion
   }
}
