using Microsoft.AspNetCore.Mvc;

using PartsHoleAPI.DBServices;
using PartsHoleLib;
using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.Controllers
{
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
      public async Task<ActionResult<bool>> Post([FromBody] BinModel value)
      {
         if (value is null)
            return BadRequest(false);
         return Ok(await _collection.AddToDatabaseAsync(value));
      }

      // POST api/Bin/many
      // Body : BinModel[] newBins
      [HttpPost("many")]
      public async Task<ActionResult> PostMany([FromBody] BinModel[] newBins)
      {
         if (newBins is null)
            return BadRequest();
         return Ok(await _collection.AddToDatabaseAsync(newBins));
      }

      // PUT api/Bin/{id}
      // Body : BinModel updatedBin
      [HttpPut("{id:length(24)}")]
      public async Task<ActionResult> Put(string id, [FromBody] BinModel updatedBin)
      {
         if (string.IsNullOrEmpty(id))
            return BadRequest(false);
         if (id.Length != 24)
            return BadRequest(false);
         return Ok(await _collection.UpdateDatabaseAsync(id, updatedBin));
      }

      // DELETE api/Bin/{id}
      [HttpDelete("{id:length(24)}")]
      public async Task<ActionResult> Delete(string id)
      {
         if (string.IsNullOrEmpty(id))
            return BadRequest(false);
         if (id.Length != 24)
            return BadRequest(false);
         return Ok(await _collection.DeleteFromDatabaseAsync(id));
      }

      // DELETE api/Bin/many
      // Body : string[] ids
      [HttpDelete("many")]
      public async Task<ActionResult> DeleteMany(string[] ids)
      {
         if (ids is null)
            return BadRequest(false);
         if (ids.Length == 0)
            return BadRequest(false);
         return Ok(await _collection.DeleteFromDatabaseAsync(ids));
      }
   }
}
