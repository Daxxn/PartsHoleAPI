using Microsoft.Extensions.Options;

using MongoDB.Driver;
using PartsHoleAPI.DBServices.Interfaces;
using PartsHoleAPI.Utils;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.DBServices
{
    public class PartNumberService : IPartNumberService
    {
        #region Local Props
        private readonly IMongoCollection<IPartNumber> _partNumberCollection;
        #endregion

        #region Constructors
        public PartNumberService(IOptions<DatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var db = client.GetDatabase(settings.Value.DatabaseName);
            _partNumberCollection = db.GetCollection<IPartNumber>(settings.Value.UsersCollection);
        }

        public IMongoCollection<IPartNumber> Collection { get; init; }

        public Task<IEnumerable<bool>?> AddToDatabaseAsync(IEnumerable<IPartNumber> data) => throw new NotImplementedException();
        public Task<bool> AddToDatabaseAsync(IPartNumber data) => throw new NotImplementedException();
        public Task<int> DeleteFromDatabaseAsync(string[] ids) => throw new NotImplementedException();
        public Task<bool> DeleteFromDatabaseAsync(string id) => throw new NotImplementedException();
        public Task<IEnumerable<IPartNumber>?> GetFromDatabaseAsync(string[] ids) => throw new NotImplementedException();
        public Task<IPartNumber?> GetFromDatabaseAsync(string id) => throw new NotImplementedException();
        public Task<IEnumerable<bool>?> UpdateDatabaseAsync(IEnumerable<IPartNumber> data) => throw new NotImplementedException();
        public Task<bool> UpdateDatabaseAsync(string id, IPartNumber data) => throw new NotImplementedException();
        #endregion

        #region Methods

        #endregion

        #region Full Props

        #endregion
    }
}
