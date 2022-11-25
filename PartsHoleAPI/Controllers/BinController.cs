using Microsoft.AspNetCore.Mvc;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

using PartsHoleRestLibrary.Responses;
using MongoDB.Bson;
using PartsHoleAPI.DBServices.Interfaces;

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
   public async Task<ActionResult<BinModel?>> Get(string id)
   {
      if (string.IsNullOrEmpty(id))
         return BadRequest();
      return Ok(await _collection.GetFromDatabaseAsync(id));
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
         return BadRequest(new APIResponse<bool>(false, "Unable to parse Bin from body."));
      return Ok(new APIResponse<bool>(await _collection.AddToDatabaseAsync(value), "POST"));
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
      if (newBins is null)
         return BadRequest(new APIResponse<IEnumerable<bool>?>("POST", "No Bins found."));
      var results = await _collection.AddToDatabaseAsync(newBins);
      if (results is null)
         return BadRequest(new APIResponse<IEnumerable<bool>?>("POST", "Failed to create Bins."));
      return Ok(new APIResponse<IEnumerable<bool>?>(results, "POST"));
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
      if (updatedBin is null)
         return BadRequest(new APIResponse<bool>(false, "PUT", "Method body not found."));
      if (updatedBin._id.Length != 24)
         return BadRequest(new APIResponse<bool>(false, "PUT", "ID not found."));
      return Ok(new APIResponse<bool>(await _collection.UpdateDatabaseAsync(updatedBin._id, updatedBin), "PUT"));
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
      if (id is null)
         return BadRequest(new APIResponse<bool>(false, "PUT", "ID not found."));
      if (id.Length != 24)
         return BadRequest(new APIResponse<bool>(false, "PUT", "ID not valid."));
      return Ok(new APIResponse<bool>(await _collection.DeleteFromDatabaseAsync(id), "DELETE"));
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
      if (ids is null)
         return BadRequest(new APIResponse<int>(0, "DELETE", "No ids found."));
      if (ids.Length == 0)
         return BadRequest(new APIResponse<int>(0, "DELETE", "No ids found."));
      return Ok(new APIResponse<int>(await _collection.DeleteFromDatabaseAsync(ids), "DELETE"));
   }
}
