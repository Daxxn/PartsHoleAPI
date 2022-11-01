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

      public async Task<IPartModel?> AddToDatabaseAsync(IPartModel data)
      {
         var filter = Builders<IPartModel>.Filter.Eq("Id", data.Id);
         if (await Collection.Find(filter).FirstOrDefaultAsync() is null)
         {
            await Collection.InsertOneAsync(data);
         }
         return data;
      }
      public async Task<IEnumerable<IPartModel>?> AddToDatabaseAsync(IEnumerable<IPartModel> data)
      {
         var ids = data.Select(x => x.Id).ToList();
         var result = await Collection.FindAsync(part => ids.Contains(part.Id));
         if (result is null) return Enumerable.Empty<IPartModel>();
         if (result.ToList().Count > 0) return Enumerable.Empty<IPartModel>();
         await Parallel.ForEachAsync(data, async (part, token) =>
         {
            await AddToDatabaseAsync(part);
         });
         return data;
      }

      public async Task UpdateDatabaseAsync(string id, IPartModel data)
      {
         var filter = Builders<IPartModel>.Filter.Where(x => id == x.Id);
         var result = await Collection.FindAsync(filter);
         if (result is null)
         {
            await AddToDatabaseAsync(data);
            return;
         }
         await Collection.ReplaceOneAsync(filter, data);
      }
      public async Task UpdateDatabaseAsync(IEnumerable<IPartModel> data)
      {
         await Parallel.ForEachAsync(data, async (d, token) =>
         {
            if (token.IsCancellationRequested) return;
            await UpdateDatabaseAsync(d.Id, d);
         });
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
