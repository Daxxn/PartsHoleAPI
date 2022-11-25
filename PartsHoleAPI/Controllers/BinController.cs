using Microsoft.AspNetCore.Mvc;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

using PartsHoleRestLibrary.Responses;
using MongoDB.Bson;
using PartsHoleAPI.DBServices.Interfaces;
using PartsHoleAPI.Utils;

namespace PartsHoleAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BinController : ControllerBase
{
   private readonly ICollectionService<BinModel> _collection;
   private readonly ILogger<BinController> _logger;
   public BinController(
      ILogger<BinController> logger,
      ICollectionService<BinModel> collection
      )
   {
      _collection = collection;
      _logger = logger;
   }

   /// <summary>
   /// Gets an <see cref="BinModel"/> based on the given <see cref="ObjectId"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>GET</term>
   ///      <description>api/bin/{<paramref name="id"/>}</description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="id"><see cref="ObjectId"/> to search for.</param>
   /// <returns>BadRequest if unable to get Bin. Otherwise Returns the <see cref="BinModel"/>.</returns>
   [HttpGet("{id:length(24)}")]
   public async Task<ActionResult<APIResponse<BinModel>>> Get(string id)
   {
      try
      {
         if (string.IsNullOrEmpty(id))
         {
            _logger.ApiLogWarn("GET", "api/bin/{id}", "Provided id was null.");
            return BadRequest(new APIResponse<BinModel>("GET", "Provided id was null."));
         }
         return Ok(await _collection.GetFromDatabaseAsync(id));
      }
      catch (Exception e)
      {
         _logger.ApiLogError("GET", "api/bin/{id}", "Internal Error", e);
         throw;
      }
   }

   /// <summary>
   /// Create a new <see cref="BinModel"/> and store it in the database.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/bin</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="BinModel"/> <paramref name="value"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="value">The new <see cref="BinModel"/> to create.</param>
   /// <returns>True if successful. Otherwise False.</returns>
   [HttpPost]
   public async Task<ActionResult<APIResponse<bool>>> Post([FromBody] BinModel value)
   {
      if (value is null)
      {
         _logger.ApiLogWarn("POST", "api/bin", "Provided Bin was null.");
         return BadRequest(new APIResponse<bool>(false, "Unable to parse Bin from body."));
      }
      try
      {
         if (await _collection.AddToDatabaseAsync(value))
         {
            _logger.ApiLogInfo("POST", "api/bin", "Successfuly add Bin to database.");
            return Ok(new APIResponse<bool>(true, "POST"));
         }
         _logger.ApiLogWarn("POST", "api/bin", "Unable to add Bin.");
         return BadRequest(new APIResponse<bool>(false, "POST", "Unable to add Bin."));
      }
      catch (Exception e)
      {
         _logger.ApiLogError("POST", "api/bin", "Internal Error", e);
         throw;
      }
   }

   /// <summary>
   /// Creates many <see cref="BinModel"/>s based on the data from the message body.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/bin/many</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="List{T}"/> of <see cref="BinModel"/> <paramref name="newBins"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="newBins"><see cref="BinModel"/> array from the message body.</param>
   /// <returns>A matching array of <see cref="bool"/> for each item. (True if successful.)</returns>
   [HttpPost("many")]
   public async Task<ActionResult<APIResponse<IEnumerable<bool>?>>> PostMany([FromBody] BinModel[] newBins)
   {
      try
      {
         if (newBins is null)
         {
            _logger.ApiLogWarn("POST", "api/bin/many", "No Bins found.");
            return BadRequest(new APIResponse<IEnumerable<bool>?>("POST", "No Bins found."));
         }
         _logger.ApiLogDebug("POST", "api/bin/many", $"Adding {newBins.Length} to the database.");
         var results = await _collection.AddToDatabaseAsync(newBins);
         if (results is null)
         {
            _logger.ApiLogWarn("POST", "api/bin/many", "Unable to create Bins.");
            return BadRequest(new APIResponse<IEnumerable<bool>?>("POST", "Unable to create Bins."));
         }
         _logger.ApiLogInfo("POST", "api/bin/many", $"Successfuly added {newBins.Length} to the database.");
         return Ok(new APIResponse<IEnumerable<bool>?>(results, "POST"));
      }
      catch (Exception e)
      {
         _logger.ApiLogError("POST", "api/bin/many", "Internal Error", e);
         throw;
      }
   }

   /// <summary>
   /// Update an <see cref="BinModel"/> with an <paramref name="updatedBin"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>PUT</term>
   ///      <description>api/bin</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="BinModel"/> <paramref name="updatedBin"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="updatedBin">the updated <see cref="BinModel"/>.</param>
   /// <returns>True if successful, otherwise false.</returns>
   [HttpPut]
   public async Task<ActionResult<APIResponse<bool>>> Put([FromBody] BinModel updatedBin)
   {
      try
      {
         if (updatedBin is null)
         {
            _logger.ApiLogWarn("PUT", "api/bin", "Body not found.");
            return BadRequest(new APIResponse<bool>(false, "PUT", "Body not found."));
         }
         if (await _collection.UpdateDatabaseAsync(updatedBin._id, updatedBin))
         {
            _logger.ApiLogInfo("PUT", "api/bin", $"Successfuly updated bin {updatedBin._id}");
            return Ok(new APIResponse<bool>(true, "PUT"));
         }
         _logger.ApiLogWarn("PUT", "api/bin", $"Unable to update bin {updatedBin._id}");
         return BadRequest(new APIResponse<bool>(false, "PUT", $"Unable to update bin {updatedBin._id}"));
      }
      catch (Exception e)
      {
         _logger.ApiLogError("PUT", "api/bin", "Internal Error", e);
         throw;
      }
   }

   /// <summary>
   /// Delete an <see cref="BinModel"/> based on the <see cref="ObjectId"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>DELETE</term>
   ///      <description>api/bin/{<paramref name="id"/>}</description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="id"><see cref="ObjectId"/> of the model to delete.</param>
   /// <returns>True if successful, otherwise false.</returns>
   [HttpDelete("{id:length(24)}")]
   public async Task<ActionResult<APIResponse<bool>>> Delete(string id)
   {
      try
      {
         if (await _collection.DeleteFromDatabaseAsync(id))
         {
            _logger.ApiLogInfo("DELETE", "api/bin/{id}", $"Successfuly deleted bin {id} from database.");
            return Ok(new APIResponse<bool>(true, "DELETE"));
         }
         _logger.ApiLogWarn("DELETE", "api/bin/{id}", $"Unable to delete bin {id} from database.");
         return BadRequest(new APIResponse<bool>(false, "DELETE", $"Unable to delete bin {id} from database."));
      }
      catch (Exception e)
      {
         _logger.ApiLogError("DELETE", "api/bin/{id}", "Internal Error", e);
         throw;
      }
   }

   /// <summary>
   /// Delete multiple <see cref="BinModel"/>s from a list of <see cref="ObjectId"/>s.
   /// <list type="table">
   ///   <item>
   ///      <term>DELETE</term>
   ///      <description>api/bin/many</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="List{T}"/> of <see cref="ObjectId"/>s <paramref name="ids"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="ids"><see cref="List{T}"/> of <see cref="ObjectId"/>s to delete.</param>
   /// <returns>Number (<see cref="int"/>) of deleted <see cref="BinModel"/>s.</returns>
   [HttpDelete("many")]
   public async Task<ActionResult<APIResponse<int>>> DeleteMany(string[] ids)
   {
      try
      {
         var deleteCount = await _collection.DeleteFromDatabaseAsync(ids);
         if (deleteCount > 0)
         {
            _logger.ApiLogInfo("DELETE", "api/bin/many", $"Successfuly deleted {ids.Length} from database.");
            return Ok(new APIResponse<int>(deleteCount, "DELETE"));
         }
         _logger.ApiLogWarn("DELETE", "api/bin/many", "Unable to delete ids from the database.");
         return BadRequest(new APIResponse<int>("DELETE", "Unable to delete ids from the database."));
      }
      catch (Exception e)
      {
         _logger.ApiLogError("DELETE", "api/bin/many", "Internal Error", e);
         throw;
      }
   }
}
