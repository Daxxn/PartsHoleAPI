using Microsoft.Extensions.Options;

using MongoDB.Driver;

using PartsHoleAPI.Utils;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.DBServices
{
   public class PartsCollection : ICollectionService<IPartModel>
   {
      #region Local Props
      public IMongoCollection<IPartModel> Collection { get; init; }
      #endregion

      #region Constructors
      public PartsCollection(IOptions<DatabaseSettings> settings)
      {
         var client = new MongoClient(settings.Value.ConnectionString);
         Collection = client
            .GetDatabase(settings.Value.Name)
            .GetCollection<IPartModel>(settings.Value.PartsCollection);
      }
      #endregion

      #region Methods
      public async Task<IPartModel?> GetFromDatabaseAsync(string id)
      {
         //var filter = Builders<IPartModel>.Filter.Eq("_id", id);
         var result = await Collection.FindAsync(part => part.Id == id);
         if (result is null) return null;
         var parts = await result.ToListAsync();
         if (parts is null) return null;
         if (parts.Count > 1) throw new Exception("Multiple parts found with that ID. Something is horribly wrong!!");
         return parts[0];
      }
      public async Task<IEnumerable<IPartModel>?> GetFromDatabaseAsync(string[] ids)
      {
         //var filter = Builders<IIPartModel>.Filter.Where(x => ids.Contains(x.Id));
         var result = await Collection.FindAsync(part => ids.Contains(part.Id));
         return result is null ? null : (IEnumerable<IPartModel>)await result.ToListAsync();
      }

      public async Task<bool> AddToDatabaseAsync(IPartModel data)
      {
         var filter = Builders<IPartModel>.Filter.Eq("Id", data.Id);
         if (await Collection.Find(filter).FirstOrDefaultAsync() is null)
         {
            await Collection.InsertOneAsync(data);
            return true;
         }
         return false;
      }
      public async Task<IEnumerable<bool>?> AddToDatabaseAsync(IEnumerable<IPartModel> data)
      {
         var status = new List<bool>();
         var partData = data.ToList();
         var ids = data.Select(x => x.Id).ToList();
         var result = await Collection.FindAsync(part => ids.Contains(part.Id));
         if (result is null) return null;
         if (result.ToList().Count > 0) return null;
         await Parallel.ForEachAsync(data, async (part, token) =>
         {
            if (token.IsCancellationRequested) return;
            var success = await AddToDatabaseAsync(part);
            var index = partData.IndexOf(part);
            status.Insert(index, success);
         });
         return status;
      }

      public async Task<bool> UpdateDatabaseAsync(string id, IPartModel data)
      {
         var filter = Builders<IPartModel>.Filter.Where(x => id == x.Id);
         var result = await Collection.FindAsync(filter);
         if (result is null)
         {
            return await AddToDatabaseAsync(data);
         }
         var replaceResult = await Collection.ReplaceOneAsync(filter, data);
         return replaceResult is null ? false : replaceResult.ModifiedCount > 0;
      }
      public async Task<IEnumerable<bool>?> UpdateDatabaseAsync(IEnumerable<IPartModel> data)
      {
         var results = new List<bool>();
         var partData = data.ToList();
         await Parallel.ForEachAsync(data, async (d, token) =>
         {
            if (token.IsCancellationRequested) return;
            var success = await UpdateDatabaseAsync(d.Id, d);
            var index = partData.IndexOf(d);
            results.Insert(index, success);
         });
         return results;
      }

      public async Task<bool> DeleteFromDatabaseAsync(string id)
      {
         var result = await Collection.DeleteOneAsync((p) => p.Id == id);
         return result is null ? false : result.DeletedCount > 0;
      }
      public async Task<int> DeleteFromDatabaseAsync(string[] ids)
      {
         var result = await Collection.DeleteManyAsync((p) => ids.Contains(p.Id));
         return result is null ? 0 : (int)result.DeletedCount;
      }
      #endregion

      #region Full Props

      #endregion
   }
}
