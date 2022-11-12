using Microsoft.AspNetCore.Mvc;

using PartsHoleAPI.DBServices;
using PartsHoleRestLibrary.Requests;
using PartsHoleRestLibrary.Responses;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BinController : ControllerBase
{
   private readonly ICollectionService<IBinModel> _collection;
   private readonly ILogger<BinController> _logger;
   public BinController(
      ILogger<BinController> logger,
      ICollectionService<IBinModel> collection
      )
   {
      _collection = collection;
      _logger = logger;
   }

   // GET: api/Bin
   [HttpGet]
   public ActionResult<string> Get()
   {
      _logger.Log(LogLevel.Debug, "Attempt to call generic GET method.");
      return Ok("Not allowed. A BIN ID is required.");
   }

   // GET api/Bin/{id}
   [HttpGet("{id:length(24)}")]
   public async Task<ActionResult<IBinModel?>> Get(string id)
   {
      if (string.IsNullOrEmpty(id))
         return BadRequest();
      return Ok(await _collection.GetFromDatabaseAsync(id));
   }

   // POST api/Bin
   [HttpPost]
   public async Task<ActionResult<APIResponse<bool>>> Post([FromBody] BinModel value)
   {
      if (value is null)
         return BadRequest(new APIResponse<bool>(false, "Unable to parse Bin from body."));
      return Ok(new APIResponse<bool>(await _collection.AddToDatabaseAsync(value), "POST"));
   }

   // POST api/Bin/many
   // Body : BinModel[] newBins
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

   // PUT api/Bin/{id}
   // Body : BinModel updatedBin
   [HttpPut("{id:length(24)}")]
   public async Task<ActionResult<APIResponse<bool>>> Put(string id, [FromBody] BinModel updatedBin)
   {
      if (updatedBin is null)
         return BadRequest(new APIResponse<bool>(false, "PUT", "Bin not found."));
      if (id.Length != 24)
         return BadRequest(new APIResponse<bool>(false, "PUT", "ID not found."));
      return Ok(new APIResponse<bool>(await _collection.UpdateDatabaseAsync(id, updatedBin), "PUT"));
   }

   // DELETE api/Bin/{id}
   [HttpDelete("{id:length(24)}")]
   public async Task<ActionResult<APIResponse<bool>>> Delete(string id)
   {
      if (id is null)
         return BadRequest(new APIResponse<bool>(false, "PUT", "ID not found."));
      if (id.Length != 24)
         return BadRequest(new APIResponse<bool>(false, "PUT", "ID not valid."));
      return Ok(new APIResponse<bool>(await _collection.DeleteFromDatabaseAsync(id), "DELETE"));
   }

   // DELETE api/Bin/many
   // Body : string[] ids
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
