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
        private readonly IMongoCollection<PartNumber> _partNumberCollection;
        #endregion

        #region Constructors
        public PartNumberService(IOptions<DatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var db = client.GetDatabase(settings.Value.DatabaseName);
            _partNumberCollection = db.GetCollection<PartNumber>(settings.Value.UsersCollection);
        }

        public IMongoCollection<PartNumber> Collection { get; init; }

        public Task<IEnumerable<bool>?> AddToDatabaseAsync(IEnumerable<PartNumber> data) => throw new NotImplementedException();
        public Task<bool> AddToDatabaseAsync(PartNumber data) => throw new NotImplementedException();
        public Task<int> DeleteFromDatabaseAsync(string[] ids) => throw new NotImplementedException();
        public Task<bool> DeleteFromDatabaseAsync(string id) => throw new NotImplementedException();
        public Task<IEnumerable<PartNumber>?> GetFromDatabaseAsync(string[] ids) => throw new NotImplementedException();
        public Task<PartNumber?> GetFromDatabaseAsync(string id) => throw new NotImplementedException();
        public Task<IEnumerable<bool>?> UpdateDatabaseAsync(IEnumerable<PartNumber> data) => throw new NotImplementedException();
        public Task<bool> UpdateDatabaseAsync(string id, PartNumber data) => throw new NotImplementedException();
        #endregion

        #region Methods

        #endregion

        #region Full Props

        #endregion
    }
}
