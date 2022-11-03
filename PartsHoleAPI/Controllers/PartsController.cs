using Microsoft.AspNetCore.Mvc;

using PartsHoleAPI.DBServices;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class PartsController : ControllerBase
   {
      #region Props
      private readonly ICollectionService<IPartModel> _partsCollection;
      private readonly ILogger<PartsController> _logger;
      #endregion

      #region Constructors
      public PartsController(ILogger<PartsController> logger, ICollectionService<IPartModel> partsCollection)
      {
         _partsCollection = partsCollection;
         _logger = logger;
      }
      #endregion

      #region API Methods
      // GET: api/<PartsController>
      [HttpGet]
      public ActionResult<string> Get()
      {
         _logger.Log(LogLevel.Debug, "Attempt to call generic GET method.");
         return Ok("Not allowed. A part ID is required.");
      }

      // GET api/<PartsController>/5
      [HttpGet("{id:length(24)}")]
      public async Task<ActionResult<IPartModel?>> Get(string id)
      {
         if (string.IsNullOrEmpty(id)) return BadRequest();
         return Ok(await _partsCollection.GetFromDatabaseAsync(id));
      }

      // POST api/<PartsController>
      // Body : PartModel newPart
      [HttpPost]
      public async Task<ActionResult> Post([FromBody] PartModel? value)
      {
         if (value is null) return BadRequest(false);
         return Ok(await _partsCollection.AddToDatabaseAsync(value));
      }

      // POST api/<PartsController>/many
      // Body : PartModel[] newParts
      [HttpPost("many")]
      public async Task<ActionResult> PostMany([FromBody] PartModel[] parts)
      {
         if (parts is null) return BadRequest();
         return Ok(await _partsCollection.AddToDatabaseAsync(parts));
      }

      // PUT api/<PartsController>/{id}
      // Body : PartModel updatedPart
      [HttpPut("{id:length(24)}")]
      public async Task<ActionResult> Put(string id, [FromBody] PartModel value)
      {
         if (string.IsNullOrEmpty(id)) return BadRequest(false);
         if (id.Length != 24) return BadRequest(false);
         return Ok(await _partsCollection.UpdateDatabaseAsync(id, value));
      }

      // DELETE api/<PartsController>/{id}
      [HttpDelete("{id:length(24)}")]
      public async Task<ActionResult> Delete(string id)
      {
         if (string.IsNullOrEmpty(id)) return BadRequest(false);
         if (id.Length != 24) return BadRequest(false);
         return Ok(await _partsCollection.DeleteFromDatabaseAsync(id));
      }

      // DELETE api/<PartsController>/many
      // Body : string[] ids
      [HttpDelete("many")]
      public async Task<ActionResult> DeleteMany(string[] ids)
      {
         if (ids is null)
            return BadRequest(false);
         if (ids.Length == 0)
            return BadRequest(false);
         return Ok(await _partsCollection.DeleteFromDatabaseAsync(ids));
      }
      #endregion
   }
}
