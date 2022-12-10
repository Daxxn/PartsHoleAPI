using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Driver;

using PartsHoleAPI.DBServices.Interfaces;
using PartsHoleAPI.Utils;

using PartsHoleLib;
using PartsHoleLib.Enums;
using PartsHoleLib.Interfaces;

using PartsHoleRestLibrary.Exceptions;
using PartsHoleRestLibrary.Requests;

namespace PartsHoleAPI.DBServices
{
   public class PartNumberService : IPartNumberService
   {
      #region Local Props
      private readonly IUserService _userService;
      private readonly IPartService _partsService;
      public IMongoCollection<PartNumber> Collection { get; init; }
      #endregion

      #region Constructors
      public PartNumberService(IOptions<DatabaseSettings> settings, IUserService userService, IPartService partsService)
      {
         _userService = userService;
         _partsService = partsService;
         var client = new MongoClient(settings.Value.ConnectionString);
         var db = client.GetDatabase(settings.Value.DatabaseName);
         Collection = db.GetCollection<PartNumber>(settings.Value.GetCollection<PartNumber>());
      }
      #endregion

      #region Methods

      #region Part Number Generation
      public async Task<PartNumber> GeneratePartNumberAsync(PartNumberRequestModel requestData)
      {
         var foundUser = await _userService.GetFromDatabaseAsync(requestData.UserId);
         if (foundUser == null)
            throw new ModelNotFoundException("UserModel", "User not found.");
         foundUser.PartNumbers ??= new();
         var allPartNumbers = new PartNumberCollection(
            (await Collection.FindAsync(pn => foundUser.PartNumbers.Contains(pn._id))).ToEnumerable()
         );

         var newPN = allPartNumbers.New(requestData.FullCategory);
         await Collection.InsertOneAsync(newPN);
         await _userService.AppendModelToUserAsync(requestData.UserId, newPN._id, ModelIDSelector.PARTNUMBERS);
         return newPN;
      }
      #endregion

      public async Task<PartNumber?> GetFromDatabaseAsync(string id) =>
         (await Collection.FindAsync(pn => pn._id == id)).FirstOrDefault();
      public async Task<IEnumerable<PartNumber>?> GetFromDatabaseAsync(string[] ids)
      {
         var idList = ids.ToList();
         return (await Collection.FindAsync(pn => idList.Contains(pn._id))).ToEnumerable();
      }

      public async Task<bool> AddToDatabaseAsync(PartNumber data)
      {
         var foundPNs = (await Collection.FindAsync(p => p.PartID == data.PartID)).ToList();
         if (foundPNs.Any())
         {
            return foundPNs.Count > 1
               ? throw new DatabaseDesyncException("More than one ID found in PartNumber database.", foundPNs.Select(x => x.ToString()))
               : false;
         }
         data._id ??= ObjectId.GenerateNewId().ToString();
         await Collection.InsertOneAsync(data);
         return true;
      }
      public async Task<IEnumerable<bool>?> AddToDatabaseAsync(IEnumerable<PartNumber> data)
      {
         var dataList = data.ToList();
         var numbers = dataList.Select(x => x.ToString());
         var foundPNs = (await Collection.FindAsync(p => numbers.Contains(p.ToString()))).ToList();

         List<bool> successList = new(dataList.Count);

         if (foundPNs.Any())
         {
            foreach (var foundPN in foundPNs)
            {
               var index = dataList.IndexOf(foundPN);
               successList.Insert(index, false);
            }
         }

         await Parallel.ForEachAsync(data, async (d, token) =>
         {
            try
            {
               await Collection.InsertOneAsync(d);
               var index = dataList.IndexOf(d);
               successList.Insert(index, true);
            }
            catch (Exception)
            {
               var index = dataList.IndexOf(d);
               successList.Insert(index, false);
            }
         });

         return successList;
      }

      public async Task<bool> UpdateDatabaseAsync(string id, PartNumber data)
      {
         if ((await Collection.FindAsync(pn => pn._id == id)).FirstOrDefault() != null)
         {
            var result = await Collection.ReplaceOneAsync((pn) => pn._id == id, data);
            return result.ModifiedCount == 1;
         }
         return false;
      }
      public async Task<IEnumerable<bool>?> UpdateDatabaseAsync(IEnumerable<PartNumber> data)
      {
         var dataList = data.ToList();
         List<bool> successList = new(dataList.Count);

         await Parallel.ForEachAsync(dataList, async (d, token) =>
         {
            var result = await Collection.ReplaceOneAsync(pn => pn._id == d._id, d, cancellationToken: token);
            var index = dataList.IndexOf(d);
            if (result.ModifiedCount == 1)
            {
               successList.Insert(index, true);
            }
            successList.Insert(index, false);
         });
         return successList;
      }

      public async Task<bool> DeleteFromDatabaseAsync(string id)
      {
         return (await Collection.DeleteOneAsync(id)).DeletedCount == 1;
         // Need to add Cookies to make this easier.
         //var foundPartNumber = await GetFromDatabaseAsync(id);
         //if (foundPartNumber == null)
         //   return false;
         //if ((await Collection.DeleteOneAsync(id)).DeletedCount == 1)
         //{
         //   var matchingParts = await _partsService.SearchForParts("Reference", foundPartNumber.ToString());
         //   if (matchingParts != null)
         //   {
         //      matchingParts.ToList().ForEach(part => { part.Reference = null; });
         //      await _partsService.UpdateDatabaseAsync(matchingParts);
         //   }
         //}
      }

      public async Task<int> DeleteFromDatabaseAsync(string[] ids)
      {
         var idList = ids.ToList();
         return (int)(await Collection.DeleteManyAsync((pn) => idList.Contains(pn._id))).DeletedCount;
      }
      #endregion

      #region Full Props

      #endregion
   }
}
