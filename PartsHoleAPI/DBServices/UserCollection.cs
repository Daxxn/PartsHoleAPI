﻿using Microsoft.Extensions.Options;

using MongoDB.Driver;

using PartsHoleAPI.Utils;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.DBServices
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
      public async Task<IUserModel?> GetFromDatabaseAsync(string id)
      {
         var result = await Collection.FindAsync(x => x.Id == id);
         if (result == null)
            return null;
         var user = await result.FirstOrDefaultAsync();
         return user;
      }
      public async Task<IUserModel?> AddToDatabaseAsync(IUserModel data)
      {
         var result = await Collection.FindAsync(x => x.Id == data.Id);
         if (result is null) return null;

         var foundUsers = await result.ToListAsync();
         if (foundUsers.Count == 0)
         {
            await Collection.InsertOneAsync(data);
         }
         return data;
      }
      public async Task UpdateDatabaseAsync(string id, IUserModel data)
      {
         var filter = Builders<IUserModel>.Filter.Where((u) => u.Id == id);
         await Collection.ReplaceOneAsync(filter, data);
         //var foundUser = Collection.Find(x => x.Id == data.Id).FirstOrDefault();
         //if (foundUser != null)
         //{
         //}
      }
      public async Task<bool> DeleteFromDatabaseAsync(string id)
      {
         var filter = Builders<IUserModel>.Filter.Where((u) => u.Id == id);
         var result = await Collection.DeleteOneAsync(filter);
         return result != null && result?.DeletedCount > 0;
      }
      #endregion

      #region Full Props

      #endregion
   }
}
