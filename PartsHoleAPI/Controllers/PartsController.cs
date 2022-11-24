using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;
using PartsHoleAPI.DBServices.Interfaces;
using PartsHoleLib;
using PartsHoleLib.Interfaces;

using PartsHoleRestLibrary.Responses;

namespace PartsHoleAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PartsController : ControllerBase
{
   #region Props
   private readonly ICollectionService<IPartModel> _collection;
   private readonly ILogger<PartsController> _logger;
   #endregion

   #region Constructors
   public PartsController(
      ILogger<PartsController> logger,
      ICollectionService<IPartModel> partsCollection
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
   public async Task<ActionResult<IPartModel?>> Get(string id)
   {
      if (string.IsNullOrEmpty(id)) return BadRequest();
      return Ok(await _collection.GetFromDatabaseAsync(id));
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
   /// <returns><see cref="List{T}"/> of <see cref="IPartModel"/>s. Null if unable.</returns>
   [HttpPost("get-many")]
   public async Task<ActionResult<APIResponse<IEnumerable<IPartModel>?>>> PostGetManyParts([FromBody] string[] ids)
   {
      if (ids is null)
         return BadRequest(new APIResponse<IEnumerable<IPartModel>?>(null, "POST", "No part IDs found."));
      var data = await _collection.GetFromDatabaseAsync(ids);
      return Ok(new APIResponse<IEnumerable<IPartModel>?>(data, "POST"));
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
   ///      <description><see cref="IPartModel"/> <paramref name="newPart"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="newPart">New <see cref="IPartModel"/> to create.</param>
   /// <returns><see langword="true"/> if successful, otherwise <see langword="false"/>.</returns>
   [HttpPost]
   public async Task<ActionResult<APIResponse<bool>>> Post([FromBody] PartModel newPart)
   {
      if (newPart is null) return BadRequest(new APIResponse<bool>(false, "POST", "No part found."));
      return Ok(new APIResponse<bool>(await _collection.AddToDatabaseAsync(newPart), "POST"));
   }

   /// <summary>
   /// Creates multiple <see cref="IPartModel"/>s.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/parts/many</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="List{T}"/> of <see cref="IPartModel"/> <paramref name="newParts"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="newParts"><see cref="List{T}"/> of new <see cref="IPartModel"/>s to create.</param>
   /// <returns><see cref="List{T}"/> of <see cref="bool"/>s where; <see langword="true"/> if successful, otherwise <see langword="false"/>.</returns>
   [HttpPost("many")]
   public async Task<ActionResult<APIResponse<IEnumerable<bool>?>>> PostMany([FromBody] PartModel[] newParts)
   {
      if (newParts is null) return BadRequest(new APIResponse<IEnumerable<bool>?>(null, "POST", "No parts found."));
      return Ok(new APIResponse<IEnumerable<bool>?>(await _collection.AddToDatabaseAsync(newParts), "POST"));
   }

   /// <summary>
   /// Updates an <see cref="IPartModel"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>PUT</term>
   ///      <description>api/parts</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="IPartModel"/> <paramref name="updatedPart"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="updatedPart">Updated <see cref="IPartModel"/> data.</param>
   /// <returns><see langword="true"/> if successful, otherwise <see langword="false"/></returns>
   [HttpPut]
   public async Task<ActionResult<APIResponse<bool>>> Put([FromBody] PartModel updatedPart)
   {
      if (string.IsNullOrEmpty(updatedPart._id)) return BadRequest(new APIResponse<bool>(false, "PUT", "ID not found."));
      if (updatedPart._id.Length != 24) return BadRequest(new APIResponse<bool>(false, "PUT", "ID not valid."));
      return Ok(new APIResponse<bool>(await _collection.UpdateDatabaseAsync(updatedPart._id, updatedPart), "PUT"));
   }

   /// <summary>
   /// Update multiple <see cref="IPartModel"/>s.
   /// <list type="table">
   ///   <item>
   ///      <term>PUT</term>
   ///      <description>api/parts/many</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="List{T}"/> <see cref="IPartModel"/> <paramref name="updatedParts"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="updatedParts"><see cref="List{T}"/> of <see cref="IPartModel"/>s to update.</param>
   /// <returns><see cref="List{T}"/> of <see cref="bool"/>s according to the index where; <see langword="true"/> if successful, otherwise <see langword="false"/>.</returns>
   [HttpPut("many")]
   public async Task<ActionResult<APIResponse<IEnumerable<bool>?>>> PutMany([FromBody] PartModel[] updatedParts)
   {
      if (updatedParts is null) return BadRequest(new APIResponse<IEnumerable<bool>?>(null, "PUT", "No parts found."));
      if (updatedParts.Length <= 0) return BadRequest(new APIResponse<IEnumerable<bool>?>(null, "PUT", "No parts in array."));
      return Ok(new APIResponse<IEnumerable<bool>?>(await _collection.UpdateDatabaseAsync(updatedParts), "PUT"));
   }

   /// <summary>
   /// Delete an <see cref="IPartModel"/> from the database.
   /// <list type="table">
   ///   <item>
   ///      <term>DELETE</term>
   ///      <description>api/parts/{<paramref name="id"/>}</description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="id">The <see cref="ObjectId"/> of the <see cref="IPartModel"/> to delete.</param>
   /// <returns><see langword="true"/> if successful, otherwise <see langword="false"/></returns>
   [HttpDelete("{id:length(24)}")]
   public async Task<ActionResult<APIResponse<bool>>> Delete(string id)
   {
      if (string.IsNullOrEmpty(id)) return BadRequest(new APIResponse<bool>(false, "DELETE", "ID not found."));
      if (id.Length != 24) return BadRequest(new APIResponse<bool>(false, "DELETE", "ID not valid."));
      return Ok(new APIResponse<bool>(await _collection.DeleteFromDatabaseAsync(id), "DELETE"));
   }

   /// <summary>
   /// Delete multiple <see cref="IPartModel"/>s from the database.
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
   /// <returns>Number (<see cref="int"/>) of <see cref="IPartModel"/>s successfully deleted.</returns>
   [HttpDelete("many")]
   public async Task<ActionResult<APIResponse<int>>> DeleteMany([FromBody] string[] ids)
   {
      if (ids is null)
         return BadRequest(new APIResponse<int>(0, "DELETE", "No IDs found."));
      if (ids.Length == 0)
         return BadRequest(new APIResponse<int>(0, "DELETE", "No IDs found."));
      return Ok(new APIResponse<int>(await _collection.DeleteFromDatabaseAsync(ids), "DELETE"));
   }
   #endregion
}
