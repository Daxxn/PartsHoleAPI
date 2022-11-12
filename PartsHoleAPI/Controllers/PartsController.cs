using Microsoft.AspNetCore.Mvc;

using PartsHoleAPI.DBServices;
using PartsHoleRestLibrary.Requests;
using PartsHoleRestLibrary.Responses;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

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
   // GET: api/Parts
   [HttpGet]
   public ActionResult<string> Get()
   {
      _logger.Log(LogLevel.Debug, "Attempt to call generic GET method.");
      return Ok("Not allowed. A part ID is required.");
   }

   // GET api/Parts/{id}
   [HttpGet("{id:length(24)}")]
   public async Task<ActionResult<IPartModel?>> Get(string id)
   {
      if (string.IsNullOrEmpty(id)) return BadRequest();
      return Ok(await _collection.GetFromDatabaseAsync(id));
   }

   // POST api/Parts
   // Body : ObjectId[] ids
   [HttpPost("get-many")]
   public async Task<ActionResult<APIResponse<IEnumerable<IPartModel>?>>> PostGetManyParts([FromBody] string[] ids)
   {
      if (ids is null)
         return BadRequest(new APIResponse<IEnumerable<IPartModel>?>(null, "POST", "No part IDs found."));
      var data = await _collection.GetFromDatabaseAsync(ids);
      return Ok(new APIResponse<IEnumerable<IPartModel>?>(data, "POST"));
   }

   // POST api/Parts
   // Body : PartModel newPart
   [HttpPost]
   public async Task<ActionResult<APIResponse<bool>>> Post([FromBody] PartModel newPart)
   {
      if (newPart is null) return BadRequest(new APIResponse<bool>(false, "POST", "No part found."));
      return Ok(new APIResponse<bool>(await _collection.AddToDatabaseAsync(newPart), "POST"));
   }

   // POST api/Parts/many
   // Body : PartModel[] newParts
   [HttpPost("many")]
   public async Task<ActionResult<APIResponse<IEnumerable<bool>?>>> PostMany([FromBody] PartModel[] newParts)
   {
      if (newParts is null) return BadRequest(new APIResponse<IEnumerable<bool>?>(null, "POST", "No parts found."));
      return Ok(new APIResponse<IEnumerable<bool>?>(await _collection.AddToDatabaseAsync(newParts), "POST"));
   }

   // PUT api/Parts/{id}
   // Body : PartModel updatedPart
   [HttpPut("{id:length(24)}")]
   public async Task<ActionResult<APIResponse<bool>>> Put(string id, [FromBody] PartModel updatedPart)
   {
      if (string.IsNullOrEmpty(id)) return BadRequest(new APIResponse<bool>(false, "PUT", "ID not found."));
      if (id.Length != 24) return BadRequest(new APIResponse<bool>(false, "PUT", "ID not valid."));
      return Ok(new APIResponse<bool>(await _collection.UpdateDatabaseAsync(id, updatedPart), "PUT"));
   }

   [HttpPut("many")]
   public async Task<ActionResult<APIResponse<IEnumerable<bool>?>>> PutMany([FromBody] PartModel[] updatedParts)
   {
      if (updatedParts is null) return BadRequest(new APIResponse<IEnumerable<bool>?>(null, "PUT", "No parts found."));
      if (updatedParts.Length <= 0) return BadRequest(new APIResponse<IEnumerable<bool>?>(null, "PUT", "No parts in array."));
      return Ok(new APIResponse<IEnumerable<bool>?>(await _collection.UpdateDatabaseAsync(updatedParts), "PUT"));
   }

   // DELETE api/Parts/{id}
   [HttpDelete("{id:length(24)}")]
   public async Task<ActionResult<APIResponse<bool>>> Delete(string id)
   {
      if (string.IsNullOrEmpty(id)) return BadRequest(new APIResponse<bool>(false, "DELETE", "ID not found."));
      if (id.Length != 24) return BadRequest(new APIResponse<bool>(false, "DELETE", "ID not valid."));
      return Ok(new APIResponse<bool>(await _collection.DeleteFromDatabaseAsync(id), "DELETE"));
   }

   // DELETE api/Parts/many
   // Body : string[] ids
   [HttpDelete("many")]
   public async Task<ActionResult<APIResponse<int>>> DeleteMany(string[] ids)
   {
      if (ids is null)
         return BadRequest(new APIResponse<int>(0, "DELETE", "No IDs found."));
      if (ids.Length == 0)
         return BadRequest(new APIResponse<int>(0, "DELETE", "No IDs found."));
      return Ok(new APIResponse<int>(await _collection.DeleteFromDatabaseAsync(ids), "DELETE"));
   }
   #endregion
}
