using Microsoft.Extensions.Options;

using MongoDB.Driver;

using PartsHoleAPI.Models;
using PartsHoleAPI.Models.Interfaces;

namespace PartsHoleAPI.Collections
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
      public async Task<IEnumerable<IPartModel>?> GetFromDatabase(string[] ids)
      {
         var filter = Builders<IPartModel>.Filter.Where(x => ids.Contains(x.Id));
         return await Collection.Find(filter).ToListAsync();
      }
      public async Task UpdateDatabase(IPartModel data)
      {
         var filter = Builders<IPartModel>.Filter.Where(x => data.Id == x.Id);

         await Collection.ReplaceOneAsync(filter, data);
      }
      public async Task UpdateDatabase(IEnumerable<IPartModel> data)
      {

      }

      public async Task<IPartModel?> GetFromDatabaseAsync(string id)
      {
         //var filter = Builders<IPartModel>.Filter.Eq("_id", id);
         return await Collection.Find(p => p.Id == id).FirstOrDefaultAsync();
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
      public async Task<IEnumerable<IPartModel>> AddToDatabase(IEnumerable<IPartModel> data) => throw new NotImplementedException();
      public async Task<bool> DeleteFromDatabase(string id) => throw new NotImplementedException();
      public async Task<int> DeleteFromDatabase(string[] id) => throw new NotImplementedException();
      #endregion

      #region Full Props

      #endregion
   }
}
