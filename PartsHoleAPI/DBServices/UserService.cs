using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Driver;

using PartsHoleAPI.Utils;

using PartsHoleLib;
using PartsHoleLib.Enums;
using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.DBServices;

public class UserService : IUserService
{
   #region Local Props
   public IMongoCollection<IUserModel> UserCollection { get; init; }
   private IMongoCollection<IPartModel> PartsCollection { get; init; }
   private IMongoCollection<IInvoiceModel> InvoicesCollection { get; init; }
   private IMongoCollection<IBinModel> BinCollection { get; set; }
   #endregion

   #region Constructors
   public UserService(IOptions<DatabaseSettings> settings)
   {
      var client = new MongoClient(settings.Value.ConnectionString);
      var db = client.GetDatabase(settings.Value.DatabaseName);
      UserCollection = db.GetCollection<IUserModel>(settings.Value.UsersCollection);
      PartsCollection = db.GetCollection<IPartModel>(settings.Value.PartsCollection);
      InvoicesCollection = db.GetCollection<IInvoiceModel>(settings.Value.InvoicesCollection);
      BinCollection = db.GetCollection<IBinModel>(settings.Value.BinsCollection);
   }
   #endregion

   #region Methods
   public async Task<IUserModel?> GetFromDatabaseAsync(string id)
   {
      var result = await UserCollection.FindAsync(x => x._id == id);
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
      if (user.Bins.Count > 0)
      {
         var binResult = await BinCollection.FindAsync((x) => user.Bins.Contains(x._id));
         if (binResult != null)
         {
            data.Bins = await binResult.ToListAsync();
         }
      }

      return data;
   }

   public async Task<bool> AddToDatabaseAsync(IUserModel data)
   {
      var result = await UserCollection.FindAsync(x => x._id == data._id);
      if (result is null)
         return false;

      var foundUsers = await result.ToListAsync();
      if (foundUsers.Count == 0)
      {
         await UserCollection.InsertOneAsync(data);
         return true;
      }
      return false;
   }

   public async Task<bool> UpdateDatabaseAsync(string id, IUserModel data)
   {
      var filter = Builders<IUserModel>.Filter.Where((u) => u._id == id);
      var result = await UserCollection.ReplaceOneAsync(filter, data);
      return result is null ? false : result.ModifiedCount > 0;
   }

   public async Task<bool> DeleteFromDatabaseAsync(string id)
   {
      var filter = Builders<IUserModel>.Filter.Where((u) => u._id == id);
      var result = await UserCollection.DeleteOneAsync(filter);
      return result != null && result?.DeletedCount > 0;
   }

   public async Task<bool> RemoveModelFromUserAsync(string userId, string modelId, ModelIDSelector selector)
   {
      var foundUser = (await UserCollection.FindAsync(user => user._id == userId)).FirstOrDefault();
      if (foundUser != null)
      {
         switch (selector)
         {
            case ModelIDSelector.PARTS:
               if (foundUser.Parts.Remove(modelId))
               {
                  //var result = await UserCollection.ReplaceOneAsync(user => user._id == userId, foundUser);
                  var update = Builders<IUserModel>.Update.Set((user) => user.Parts, foundUser.Parts);
                  var result = await UserCollection.UpdateOneAsync(user => user._id == userId, update);
                  if (result.IsAcknowledged)
                  {
                     return result.ModifiedCount > 0;
                  }
               }
               return false;
            case ModelIDSelector.INVOICE:
               if (foundUser.Invoices.Remove(modelId))
               {
                  var update = Builders<IUserModel>.Update.Set((user) => user.Invoices, foundUser.Invoices);
                  var result = await UserCollection.UpdateOneAsync(user => user._id == userId, update);
                  if (result.IsAcknowledged)
                  {
                     return result.ModifiedCount > 0;
                  }
               }
               return false;
            case ModelIDSelector.BINS:
               if (foundUser.Bins.Remove(modelId))
               {
                  var update = Builders<IUserModel>.Update.Set((user) => user.Bins, foundUser.Bins);
                  var result = await UserCollection.UpdateOneAsync(user => user._id == userId, update);
                  if (result.IsAcknowledged)
                  {
                     return result.ModifiedCount > 0;
                  }
               }
               return false;
            default:
               throw new ArgumentOutOfRangeException(nameof(selector));
         }
      }
      return false;
   }
   #endregion

   #region Full Props

   #endregion
}
