using Microsoft.Extensions.Options;

using MongoDB.Driver;

using PartsHoleAPI.Models;
using PartsHoleAPI.Models.Interfaces;

namespace PartsHoleAPI.Collections
{
   public class UserCollection : IModelService<IUserModel>
   {
      #region Local Props
      public IMongoCollection<IUserModel> Collection { get; init; }
      #endregion

      #region Constructors
      public UserCollection(IOptions<DatabaseSettings> settings)
      {
         var client = new MongoClient(settings.Value.ConnectionString);
         Collection = client
            .GetDatabase(settings.Value.Name)
            .GetCollection<IUserModel>(settings.Value.UsersCollection);
      }
      #endregion

      #region Methods
      public async Task<IUserModel> GetFromDatabaseAsync(string id)
      {
         return await Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
      }
      public async Task<IUserModel> AddToDatabaseAsync(IUserModel data)
      {
         //var filter = Builders<IUserModel>.Filter.Eq("Id", data.Id);
         var foundUsers = (await Collection.Find(x => x.Id == data.Id).ToListAsync()).Count > 0;
         if (!foundUsers)
         {
            await Collection.InsertOneAsync(data);
         }
         return data;
      }
      public async Task UpdateDatabase(IUserModel data)
      {
         var filter = Builders<IUserModel>.Filter.Eq("Id", data.Id);
         await Collection.ReplaceOneAsync(filter, data);
         //var foundUser = Collection.Find(x => x.Id == data.Id).FirstOrDefault();
         //if (foundUser != null)
         //{
         //}
      }
      public async Task<bool> DeleteFromDatabase(string id)
      {
         var filter = Builders<IUserModel>.Filter.Eq("Id", id);
         var result = await Collection.DeleteOneAsync(filter);
         return result != null && result?.DeletedCount > 0;
      }
      #endregion

      #region Full Props

      #endregion
   }
}
