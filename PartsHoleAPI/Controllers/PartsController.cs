using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;

using PartsHoleAPI.DBServices.Interfaces;
using PartsHoleAPI.Utils;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

using PartsHoleRestLibrary.Responses;

namespace PartsHoleAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PartsController : ControllerBase
{
   #region Props
   private readonly IPartService _collection;
   private readonly ILogger<PartsController> _logger;
   #endregion

   #region Constructors
   public PartsController(
      ILogger<PartsController> logger,
      IPartService partsCollection
      )
   {
      _collection = partsCollection;
      _logger = logger;
   }
   #endregion

   #region API Methods
   /// <summary>
   /// Gets an <see cref="IPartModel"/> based on the given <see cref="ObjectId"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>GET</term>
   ///      <description>api/parts/{<paramref name="id"/>}</description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="id"><see cref="ObjectId"/> to search for.</param>
   /// <returns><see cref="IPartModel"/> found. Null if unable.</returns>
   [HttpGet("{id:length(24)}")]
   public async Task<APIResponse<PartModel>> Get(string id)
   {
      try
      {
         var foundPart = await _collection.GetFromDatabaseAsync(id);
         if (foundPart == null)
         {
            _logger.ApiLogWarn("GET", "api/parts/{id}", $"Unable to find part {id}");
            return new("GET", $"Unable to find part {id}.");
         }
         _logger.ApiLogInfo("GET", "api/parts/{id}", $"Successfuly found part {id}");
         return new(foundPart, "GET");
      }
      catch (Exception e)
      {
         _logger.ApiLogError("GET", "api/parts/{id}", "Internal Error", e);
         throw;
      }
   }

   /// <summary>
   /// Get multiple parts based on a <see cref="List{T}"/> of <see cref="ObjectId"/>s.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/parts/get-many</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="List{T}"/> <see cref="ObjectId"/> <paramref name="ids"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="ids"><see cref="List{T}"/> of <see cref="ObjectId"/>s to get.</param>
   /// <returns><see cref="List{T}"/> of <see cref="PartModel"/>s. Null if unable.</returns>
   [HttpPost("get-many")]
   public async Task<APIResponse<IEnumerable<PartModel>>> PostGetManyParts([FromBody] string[] ids)
   {
      if (ids is null)
      {
         _logger.ApiLogWarn("POST", "api/parts/get-many", "No part IDs found.");
         return new("POST", "No part IDs found.");
      }
      var data = await _collection.GetFromDatabaseAsync(ids);
      if (data == null)
      {
         _logger.ApiLogWarn("POST", "api/parts/get-many", "No part models found.");
         return new("POST", "No part models found.");
      }
      return new(data, "POST");
   }

   /// <summary>
   /// Creates a new <see cref="IPartModel"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/parts</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="PartModel"/> <paramref name="newPart"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="newPart">New <see cref="PartModel"/> to create.</param>
   /// <returns><see langword="true"/> if successful, otherwise <see langword="false"/>.</returns>
   [HttpPost]
   public async Task<APIResponse<bool>> Post([FromBody] PartModel newPart)
   {
      try
      {
         if (newPart is null)
         {
            _logger.ApiLogWarn("POST", "api/parts", "Body was null.");
            return new("POST", "Body is null.");
         }
         if (await _collection.AddToDatabaseAsync(newPart))
         {
            _logger.ApiLogInfo("POST", "api/parts", "Part successfully added to database.");
            return new(true, "POST");
         }
         _logger.ApiLogWarn("POST", "api/parts", "Unable to add part to database.");
         return new(false, "POST", "Unable to add part to database.");
      }
      catch (Exception e)
      {
         _logger.ApiLogError("POST", "api/parts", "Internal Error", e);
         throw;
      }
   }

   /// <summary>
   /// Creates multiple <see cref="PartModel"/>s.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/parts/many</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="List{T}"/> of <see cref="PartModel"/> <paramref name="newParts"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="newParts"><see cref="List{T}"/> of new <see cref="PartModel"/>s to create.</param>
   /// <returns><see cref="List{T}"/> of <see cref="bool"/>s where; <see langword="true"/> if successful, otherwise <see langword="false"/>.</returns>
   [HttpPost("many")]
   public async Task<APIResponse<IEnumerable<bool>?>> PostMany([FromBody] PartModel[] newParts)
   {
      try
      {
         if (newParts is null)
         {
            _logger.ApiLogWarn("POST", "api/parts/many", "Body was null.");
            return new("POST", "Body is null.");
         }
         var successList = await _collection.AddToDatabaseAsync(newParts);
         if (successList != null)
         {
            _logger.ApiLogInfo("POST", "api/parts/many", $"Successfully added {newParts.Length} parts to database.");
            return new(successList, "POST");
         }
         _logger.ApiLogWarn("POST", "api/parts/many", "No parts added to database.");
         return new("POST", "No parts added to database.");
      }
      catch (Exception e)
      {
         _logger.ApiLogError("POST", "api/parts/many", "Internal Error", e);
         throw;
      }
   }

   /// <summary>
   /// Updates an <see cref="PartModel"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>PUT</term>
   ///      <description>api/parts</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="PartModel"/> <paramref name="updatedPart"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="updatedPart">Updated <see cref="PartModel"/> data.</param>
   /// <returns><see langword="true"/> if successful, otherwise <see langword="false"/></returns>
   [HttpPut]
   public async Task<APIResponse<bool>> Put([FromBody] PartModel updatedPart)
   {
      try
      {
         if (string.IsNullOrEmpty(updatedPart._id))
         {
            _logger.ApiLogWarn("PUT", "api/parts", "Updated part ID was null.");
            return new(false, "PUT", "Updated part ID is null.");
         }
         if (updatedPart._id.Length != 24)
         {
            _logger.ApiLogWarn("PUT", "api/parts", "Unable to validate updated part ID.");
            return new(false, "PUT", "ID not valid.");
         }
         if (await _collection.UpdateDatabaseAsync(updatedPart._id, updatedPart))
         {
            _logger.ApiLogInfo("PUT", "api/parts", $"Successfully updated part {updatedPart._id}");
            return new(true, "PUT");
         }
         _logger.ApiLogWarn("PUT", "api/parts", "Unable to updated part model.");
         return new(false, "PUT", "Unable to updated part model.");
      }
      catch (Exception e)
      {
         _logger.ApiLogError("PUT", "api/parts", "Internal Error", e);
         throw;
      }
   }

   /// <summary>
   /// Update multiple <see cref="PartModel"/>s.
   /// <list type="table">
   ///   <item>
   ///      <term>PUT</term>
   ///      <description>api/parts/many</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="List{T}"/> <see cref="PartModel"/> <paramref name="updatedParts"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="updatedParts"><see cref="List{T}"/> of <see cref="PartModel"/>s to update.</param>
   /// <returns><see cref="List{T}"/> of <see cref="bool"/>s according to the index where; <see langword="true"/> if successful, otherwise <see langword="false"/>.</returns>
   [HttpPut("many")]
   public async Task<APIResponse<IEnumerable<bool>>> PutMany([FromBody] PartModel[] updatedParts)
   {
      try
      {
         if (updatedParts is null)
         {
            _logger.ApiLogWarn("PUT", "api/parts/many", "Body was null.");
            return new("PUT", "Body is null.");
         }
         if (updatedParts.Length <= 0)
         {
            _logger.ApiLogWarn("PUT", "api/parts/many", "No parts found in request body.");
            return new("PUT", "No parts found in request body.");
         }
         var successList = await _collection.UpdateDatabaseAsync(updatedParts);
         if (successList is null)
         {
            _logger.ApiLogWarn("PUT", "api/parts/many", "Unable to update parts.");
            return new("PUT", "Unable to update parts.");
         }
         _logger.ApiLogInfo("PUT", "api/parts/many", "Parts updated in database.");
         return new(successList, "PUT");
      }
      catch (Exception e)
      {
         _logger.ApiLogError("PUT", "api/parts/many", "Internal Error", e);
         throw;
      }
   }

   /// <summary>
   /// Delete an <see cref="PartModel"/> from the database.
   /// <list type="table">
   ///   <item>
   ///      <term>DELETE</term>
   ///      <description>api/parts/{<paramref name="id"/>}</description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="id">The <see cref="ObjectId"/> of the <see cref="PartModel"/> to delete.</param>
   /// <returns><see langword="true"/> if successful, otherwise <see langword="false"/></returns>
   [HttpDelete("{id:length(24)}")]
   public async Task<APIResponse<bool>> Delete(string id)
   {
      try
      {
         if (await _collection.DeleteFromDatabaseAsync(id))
         {
            _logger.ApiLogInfo("DELETE", "api/parts/{id}", $"Deleted part {id} from database.");
            return new(true, "DELETE", $"Deleted part { id } from database.");
         }
         _logger.ApiLogWarn("DELETE", "api/parts/{id}", $"Unable to delete part {id} database.");
         return new(false, "DELETE", $"Unable to delete part {id} database.");
      }
      catch (Exception e)
      {
         _logger.ApiLogError("PUT", "api/parts/{id}", "Internal Error", e);
         throw;
      }
   }

   /// <summary>
   /// Delete multiple <see cref="PartModel"/>s from the database.
   /// <list type="table">
   ///   <item>
   ///      <term>DELETE</term>
   ///      <description>api/parts/many</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="List{T}"/> <see cref="ObjectId"/> <paramref name="ids"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="ids"><see cref="List{T}"/> of <see cref="ObjectId"/>s to delete.</param>
   /// <returns>Number (<see cref="int"/>) of <see cref="PartModel"/>s successfully deleted.</returns>
   [HttpDelete("many")]
   public async Task<APIResponse<int>> DeleteMany([FromBody] string[] ids)
   {
      try
      {
         if (ids is null)
         {
            _logger.ApiLogWarn("DELETE", "api/parts/many", "Body was null.");
            return new("DELETE", "No IDs found.");
         }
         if (ids.Length == 0)
         {
            _logger.ApiLogWarn("DELETE", "api/parts/many", "No IDs found.");
            return new(0, "DELETE", "No IDs found.");
         }
         var deletedCount = await _collection.DeleteFromDatabaseAsync(ids);
         if (deletedCount > 0)
         {
            _logger.ApiLogInfo("DELETE", "api/parts/many", $"Deleted {deletedCount} from database");
            return new(deletedCount, "DELETE");
         }
         _logger.ApiLogInfo("DELETE", "api/parts/many", "Unable to delete parts.");
         return new("DELETE", "Unable to delete parts.");
      }
      catch (Exception e)
      {
         _logger.ApiLogError("DELETE", "api/parts/many", "Internal Error", e);
         throw;
      }
   }
   #endregion
}
