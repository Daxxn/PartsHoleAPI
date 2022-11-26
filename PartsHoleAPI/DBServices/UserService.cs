using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Driver;

using PartsHoleAPI.DBServices.Interfaces;
using PartsHoleAPI.Utils;

using PartsHoleLib;
using PartsHoleLib.Enums;
using PartsHoleLib.Interfaces;

using PartsHoleRestLibrary.Exceptions;

namespace PartsHoleAPI.DBServices;

public class UserService : IUserService
{
   #region Local Props
   public IMongoCollection<UserModel> UserCollection { get; init; }
   private IMongoCollection<PartModel> PartsCollection { get; init; }
   private IMongoCollection<InvoiceModel> InvoicesCollection { get; init; }
   private IMongoCollection<BinModel> BinCollection { get; set; }
   private IMongoCollection<PartNumber> PartNumberCollection { get; set; }
   #endregion

   #region Constructors
   public UserService(IOptions<DatabaseSettings> settings)
   {
      var client = new MongoClient(settings.Value.ConnectionString);
      var db = client.GetDatabase(settings.Value.DatabaseName);
      UserCollection = db.GetCollection<UserModel>(settings.Value.UsersCollection);
      PartsCollection = db.GetCollection<PartModel>(settings.Value.PartsCollection);
      InvoicesCollection = db.GetCollection<InvoiceModel>(settings.Value.InvoicesCollection);
      BinCollection = db.GetCollection<BinModel>(settings.Value.BinsCollection);
      PartNumberCollection = db.GetCollection<PartNumber>(settings.Value.PartNumberCollection);
   }
   #endregion

   #region Methods
   public async Task<UserModel?> GetFromDatabaseAsync(string id)
   {
      var result = await UserCollection.FindAsync(x => x._id == id);
      if (result == null)
         return null;
      var user = await result.FirstOrDefaultAsync();
      return user;
   }

   public async Task<UserData?> GetUserDataFromDatabaseAsync(UserModel user)
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
      if (user.PartNumbers.Count > 0)
      {
         var partNumbers = await PartNumberCollection.FindAsync(x => user.PartNumbers.Contains(x._id));
         if (partNumbers != null)
         {
            data.PartNumbers = await partNumbers.ToListAsync();
         }
      }

      return data;
   }

   public async Task<bool> AddToDatabaseAsync(UserModel data)
   {
      var result = await UserCollection.FindAsync(x => x._id == data._id);
      if (result is null)
         throw new ModelNotFoundException("UserModel", "User not found.");

      var foundUsers = await result.ToListAsync();
      if (foundUsers.Count == 0)
      {
         await UserCollection.InsertOneAsync(data);
         return true;
      }
      return false;
   }

   public async Task<bool> UpdateDatabaseAsync(string id, UserModel data)
   {
      var filter = Builders<UserModel>.Filter.Where((u) => u._id == id);
      var result = await UserCollection.ReplaceOneAsync(filter, data);
      return result is null ? false : result.ModifiedCount > 0;
   }

   public async Task<bool> DeleteFromDatabaseAsync(string id)
   {
      var filter = Builders<UserModel>.Filter.Where((u) => u._id == id);
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
                  var update = Builders<UserModel>.Update.Set((user) => user.Parts, foundUser.Parts);
                  var result = await UserCollection.UpdateOneAsync(user => user._id == userId, update);
                  if (result.IsAcknowledged)
                  {
                     return result.ModifiedCount > 0;
                  }
               }
               return false;
            case ModelIDSelector.INVOICES:
               if (foundUser.Invoices.Remove(modelId))
               {
                  var update = Builders<UserModel>.Update.Set((user) => user.Invoices, foundUser.Invoices);
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
                  var update = Builders<UserModel>.Update.Set((user) => user.Bins, foundUser.Bins);
                  var result = await UserCollection.UpdateOneAsync(user => user._id == userId, update);
                  if (result.IsAcknowledged)
                  {
                     return result.ModifiedCount > 0;
                  }
               }
               return false;
            case ModelIDSelector.PARTNUMBERS:
               if (foundUser.PartNumbers.Remove(modelId))
               {
                  var update = Builders<UserModel>.Update.Set((user) => user.Parts, foundUser.PartNumbers);
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
      throw new ModelNotFoundException("UserModel", "User not found.");
   }

   public async Task<bool> AppendModelToUserAsync(string userId, string modelId, ModelIDSelector selector)
   {
      var foundUser = (await UserCollection.FindAsync(user => user._id == userId)).FirstOrDefault();
      if (foundUser != null)
      {
         switch (selector)
         {
            case ModelIDSelector.PARTS:
               foundUser.Parts.Add(modelId);
               var update = Builders<UserModel>.Update.Set((user) => user.Parts, foundUser.Parts);
               var result = await UserCollection.UpdateOneAsync(user => user._id == userId, update);
               if (result.IsAcknowledged)
               {
                  return result.ModifiedCount > 0;
               }
               return false;
            case ModelIDSelector.INVOICES:
               foundUser.Invoices.Add(modelId);
               update = Builders<UserModel>.Update.Set((user) => user.Invoices, foundUser.Invoices);
               result = await UserCollection.UpdateOneAsync(user => user._id == userId, update);
               if (result.IsAcknowledged)
               {
                  return result.ModifiedCount > 0;
               }
               return false;
            case ModelIDSelector.BINS:
               foundUser.Bins.Add(modelId);
               update = Builders<UserModel>.Update.Set((user) => user.Bins, foundUser.Bins);
               result = await UserCollection.UpdateOneAsync(user => user._id == userId, update);
               if (result.IsAcknowledged)
               {
                  return result.ModifiedCount > 0;
               }
               return false;
            case ModelIDSelector.PARTNUMBERS:
               foundUser.PartNumbers.Add(modelId);
               update = Builders<UserModel>.Update.Set((user) => user.PartNumbers, foundUser.PartNumbers);
               result = await UserCollection.UpdateOneAsync(user => user._id == userId, update);
               if (result.IsAcknowledged)
               {
                  return result.ModifiedCount > 0;
               }
               return false;
            default:
               throw new ArgumentOutOfRangeException(nameof(selector));
         }
      }
      throw new ModelNotFoundException("UserModel", "User not found.");
   }
   #endregion

   #region Full Props

   #endregion
}
