using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Driver;

using PartsHoleAPI.Utils;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.DBServices;

public class UserCollection : IUserCollection
{
   #region Local Props
   public IMongoCollection<IUserModel> Collection { get; init; }
   private IMongoCollection<IPartModel> PartsCollection { get; init; }
   private IMongoCollection<IInvoiceModel> InvoicesCollection { get; init; }
   #endregion

   #region Constructors
   public UserCollection(IOptions<DatabaseSettings> settings)
   {
      var client = new MongoClient(settings.Value.ConnectionString);
      var db = client.GetDatabase(settings.Value.DatabaseName);
      Collection = db.GetCollection<IUserModel>(settings.Value.UsersCollection);
      PartsCollection = db.GetCollection<IPartModel>(settings.Value.PartsCollection);
      InvoicesCollection = db.GetCollection<IInvoiceModel>(settings.Value.InvoicesCollection);
   }
   #endregion

   #region Methods
   public async Task<IUserModel?> GetFromDatabaseAsync(string id)
   {
      var result = await Collection.FindAsync(x => x._id == id);
      if (result == null)
         return null;
      var user = await result.FirstOrDefaultAsync();
      return user;
   }

   public async Task<IUserData?> GetUserDataFromDatabaseAsync(IUserModel user)
   {
      UserData data = new UserData();

      if (user.Parts.Count > 0)
      {
         var partsResult = await PartsCollection.FindAsync((x) => user.Parts.Contains(x._id));
         if (partsResult != null)
         {
            //while (partsResult.)
            //{

            //}
            //var part = partsResult.FirstOrDefault();
            data.Parts = await partsResult.ToListAsync();
         }
      }
      if (user.Invoices.Count > 0)
      {
         var invoiceResult = await InvoicesCollection.FindAsync((x) => user.Invoices.Contains(x._id));
         if (invoiceResult != null)
         {
            data.Invoices = await invoiceResult.ToListAsync();
         }
      }
      return data;
   }

   public async Task<bool> AddToDatabaseAsync(IUserModel data)
   {
      var result = await Collection.FindAsync(x => x._id == data._id);
      if (result is null)
         return false;

      var foundUsers = await result.ToListAsync();
      if (foundUsers.Count == 0)
      {
         await Collection.InsertOneAsync(data);
         return true;
      }
      return false;
   }

   public async Task<bool> UpdateDatabaseAsync(string id, IUserModel data)
   {
      var filter = Builders<IUserModel>.Filter.Where((u) => u._id == id);
      var result = await Collection.ReplaceOneAsync(filter, data);
      return result is null ? false : result.ModifiedCount > 0;
   }

   public async Task<bool> DeleteFromDatabaseAsync(string id)
   {
      var filter = Builders<IUserModel>.Filter.Where((u) => u._id == id);
      var result = await Collection.DeleteOneAsync(filter);
      return result != null && result?.DeletedCount > 0;
   }
   #endregion

   #region Full Props

   #endregion
}
