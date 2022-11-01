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
      private readonly ICollectionService<PartModel> _partsCollection;
      private readonly ILogger<PartsController> _logger;

      public PartsController(ILogger<PartsController> logger, ICollectionService<PartModel> partsCollection)
      {
         _partsCollection = partsCollection;
         _logger = logger;
      }

      // GET: api/<PartsController>
      [HttpGet]
      public ActionResult<string> Get()
      {
         _logger.Log(LogLevel.Debug, "Attempt to call generic GET method.");
         return Ok("Not allowed. A part ID is required.");
      }

      // GET api/<PartsController>/5
      [HttpGet("{id:length(24)}")]
      public async Task<IPartModel?> Get(string id)
      {
         if (string.IsNullOrEmpty(id))
         {
            StatusCode(StatusCodes.Status400BadRequest);
            return null;
         }
         return await _partsCollection.GetFromDatabaseAsync(id);
      }

      // POST api/<PartsController>
      [HttpPost]
      public async Task Post([FromBody] PartModel? value)
      {
         if (value is null)
            return;
         await _partsCollection.AddToDatabaseAsync(value);
      }

      // PUT api/<PartsController>/5
      [HttpPut("{id}")]
      public async Task Put(int id, [FromBody] string value)
      {
      }

      // DELETE api/<PartsController>/5
      [HttpDelete("{id}")]
      public async Task Delete(int id)
      {
      }
   }
}
