using Microsoft.Extensions.Options;

using MongoDB.Driver;

using PartsHoleAPI.DBServices.Interfaces;
using PartsHoleAPI.Utils;

using PartsHoleLib;

namespace PartsHoleAPI.DBServices
{
    public class PartService : CollectionService<PartModel>, IPartService
    {
        #region Local Props
        public IUserService _userService;
        #endregion

        #region Constructors
        public PartService(IOptions<DatabaseSettings> settings, IUserService userService) : base(settings)
        {
            _userService = userService;
        }
        #endregion

        #region Methods
        public async Task<IEnumerable<PartModel>?> SearchForParts<T>(string prop, T match)
        {
            var builder = Builders<PartModel>.Filter.Eq(prop, match);
            return (await Collection.FindAsync(builder)).ToEnumerable();
        }
        #endregion

        #region Full Props

        #endregion
    }
}
