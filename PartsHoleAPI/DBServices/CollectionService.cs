﻿using Microsoft.Extensions.Options;

using MongoDB.Driver;

using PartsHoleAPI.Utils;

using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.DBServices
{
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
         Collection = client.GetDatabase(settings.Value.Name).GetCollection<T>(str);
      }
      #endregion

      #region Methods
      public async Task<T?> GetFromDatabaseAsync(string id)
      {
         var result = await Collection.FindAsync(part => part.Id == id);
         if (result is null)
            return null;
         var parts = await result.ToListAsync();
         if (parts is null)
            return null;
         if (parts.Count > 1)
            throw new Exception("Multiple parts found with that ID. Something is horribly wrong!!");
         return parts[0];
      }
      public async Task<IEnumerable<T>?> GetFromDatabaseAsync(string[] ids)
      {
         var result = await Collection.FindAsync(part => ids.Contains(part.Id));
         return result is null ? null : result.ToEnumerable();
      }
      public async Task<bool> AddToDatabaseAsync(T data)
      {
         var filter = Builders<T>.Filter.Eq("Id", data.Id);
         if (await Collection.Find(filter).FirstOrDefaultAsync() is null)
         {
            await Collection.InsertOneAsync(data);
            return true;
         }
         return false;
      }
      public async Task<IEnumerable<bool>?> AddToDatabaseAsync(IEnumerable<T> data)
      {
         var status = new List<bool>();
         var partData = data.ToList();
         var ids = data.Select(x => x.Id).ToList();
         var result = await Collection.FindAsync(part => ids.Contains(part.Id));
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
         var filter = Builders<T>.Filter.Where(x => id == x.Id);
         var result = await Collection.FindAsync(filter);
         if (result is null)
         {
            return await AddToDatabaseAsync(data);
         }
         var replaceResult = await Collection.ReplaceOneAsync(filter, data);
         return replaceResult is null ? false : replaceResult.ModifiedCount > 0;
      }
      public async Task<IEnumerable<bool>?> UpdateDatabaseAsync(IEnumerable<T> data)
      {
         var results = new List<bool>();
         var partData = data.ToList();
         await Parallel.ForEachAsync(data, async (d, token) =>
         {
            if (token.IsCancellationRequested)
               return;
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