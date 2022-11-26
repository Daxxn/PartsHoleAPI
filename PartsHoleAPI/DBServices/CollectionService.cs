using Microsoft.Extensions.Options;

using MongoDB.Driver;
using PartsHoleAPI.DBServices.Interfaces;
using PartsHoleAPI.Utils;

using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.DBServices;

public class CollectionService<T> : ICollectionService<T> where T : class, IModel
{
   #region Local Props
   public IMongoCollection<T> Collection { get; init; }
   #endregion

   #region Constructors
   public CollectionService(IOptions<DatabaseSettings> settings)
   {
      var str = settings.Value.GetCollection<T>();
      var client = new MongoClient(settings.Value.ConnectionString);
      Collection = client.GetDatabase(settings.Value.DatabaseName).GetCollection<T>(str);
   }
   #endregion

   #region Methods
   public async Task<T?> GetFromDatabaseAsync(string id)
   {
      var result = await Collection.FindAsync(part => part._id == id);
      if (result is null)
         return null;
      var parts = await result.ToListAsync();
      if (parts is null)
         return null;
      if (parts.Count > 1)
         throw new Exception("Multiple models found with that ID. Something is horribly wrong!!");
      return parts[0];
   }

   public async Task<IEnumerable<T>?> GetFromDatabaseAsync(string[] ids)
   {
      var result = await Collection.FindAsync(part => ids.Contains(part._id));
      return result is null ? null : result.ToEnumerable();
   }

   public async Task<bool> AddToDatabaseAsync(T data)
   {
      var filter = Builders<T>.Filter.Eq("Id", data._id);
      if (await Collection.Find(filter).FirstOrDefaultAsync() is null)
      {
         await Collection.InsertOneAsync(data);
         return true;
      }
      return false;
   }

   public async Task<IEnumerable<bool>?> AddToDatabaseAsync(IEnumerable<T> data)
   {
      var partData = data.ToList();
      var status = new List<bool>(partData.Count);
      var ids = data.Select(x => x._id).ToList();
      var result = await Collection.FindAsync(part => ids.Contains(part._id));
      if (result is null)
         return null;
      if (result.ToList().Count > 0)
         return null;
      await Parallel.ForEachAsync(data, async (part, token) =>
      {
         if (token.IsCancellationRequested)
            return;
         var success = await AddToDatabaseAsync(part);
         var index = partData.IndexOf(part);
         status.Insert(index, success);
      });
      return status;
   }

   public async Task<bool> UpdateDatabaseAsync(string id, T data)
   {
      var filter = Builders<T>.Filter.Where(x => id == x._id);
      var result = await Collection.FindAsync(filter);
      if (result is null) throw new Exception("Find failed.");
      if (result.FirstOrDefault() is null) return await AddToDatabaseAsync(data);
      var replaceResult = await Collection.ReplaceOneAsync(filter, data);
      return replaceResult is null ? false : replaceResult.ModifiedCount > 0;
   }

   public async Task<IEnumerable<bool>?> UpdateDatabaseAsync(IEnumerable<T> data)
   {
      var partData = data.ToList();
      var results = new List<bool>(partData.Count);
      await Parallel.ForEachAsync(data, async (d, token) =>
      {
         if (token.IsCancellationRequested)
            return;
         var success = false;
         if (!string.IsNullOrEmpty(d._id))
         {
            success = await UpdateDatabaseAsync(d._id, d);
         }
         var index = partData.IndexOf(d);
         results.Insert(index, success);
      });
      //foreach (var d in partData)
      //{
      //   var success = false;
      //   if (!string.IsNullOrEmpty(d._id))
      //   {
      //      success = await UpdateDatabaseAsync(d._id, d);
      //   }
      //   var index = partData.IndexOf(d);
      //   results.Insert(index, success);
      //}
      return results;
   }

   public async Task<bool> DeleteFromDatabaseAsync(string id)
   {
      var result = await Collection.DeleteOneAsync((p) => p._id == id);
      return result is not null && result.DeletedCount > 0;
   }

   public async Task<int> DeleteFromDatabaseAsync(string[] ids)
   {
      var result = await Collection.DeleteManyAsync((p) => ids.Contains(p._id));
      return result is null ? 0 : (int)result.DeletedCount;
   }
   #endregion

   #region Full Props

   #endregion
}
