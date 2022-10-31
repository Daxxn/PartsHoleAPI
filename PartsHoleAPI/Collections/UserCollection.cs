using MongoDB.Driver;

using PartsHoleAPI.Models;
using PartsHoleAPI.Models.Interfaces;

namespace PartsHoleAPI.Collections
{
   public class UserCollection : ICollection<IUserModel>
   {
      #region Local Props
      public IMongoClient Client { get; init; }
      #endregion

      #region Constructors
      public UserCollection(IMongoClient client) => Client = client;
      #endregion

      #region Methods
      public IUserModel GetFromDatabase()
      {
         throw new NotImplementedException();
      }
      public IUserModel GetFromDatabase(string id)
      {
         throw new NotImplementedException();
      }
      public IUserModel AddToDatabase(IUserModel data) => throw new NotImplementedException();
      public IEnumerable<IUserModel> AddToDatabase(IEnumerable<IUserModel> data) => throw new NotImplementedException();
      public IEnumerable<IUserModel> GetFromDatabase(string[] ids)
      {
         throw new NotImplementedException();
      }
      public void UpdateDatabase(IUserModel data)
      {
      }
      public void UpdateDatabase(IEnumerable<IUserModel> data) => throw new NotImplementedException();

      public bool DeleteFromDatabase(string id)
      {
          throw new NotImplementedException();
      }
      public int DeleteFromDatabase(string[] id) => throw new NotImplementedException();
      #endregion

      #region Full Props

      #endregion
   }
}
