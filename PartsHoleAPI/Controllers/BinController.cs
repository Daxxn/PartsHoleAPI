using Microsoft.AspNetCore.Mvc;

using PartsHoleAPI.DBServices;
using PartsHoleLib;

namespace PartsHoleAPI.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class BinController : ControllerBase
   {
      private readonly ICollectionService<BinModel> _collection;
      private readonly ILogger<BinController> _logger;
      public BinController(ILogger<BinController> logger, ICollectionService<BinModel> collection)
      {
         _collection = collection;
         _logger = logger;
      }

      // GET: api/<BinController>
      [HttpGet]
      public ActionResult<string> Get()
      {
         _logger.Log(LogLevel.Debug, "Attempt to call generic GET method.");
         return Ok("Not allowed. A BIN ID is required.");
      }

      // GET api/<BinController>/5
      [HttpGet("{id}")]
      public string Get(int id)
      {
         return "value";
      }

      // POST api/<BinController>
      [HttpPost]
      public void Post([FromBody] string value)
      {
      }

      //[HttpPost]
      //public async Task<ActionResult<bool[]?>> PostMany([FromBody] IEnumerable<BinModel>? data)
      //{
      //   if (data is null)
      //      return BadRequest();


      //}

      // PUT api/<BinController>/5
      [HttpPut("{id}")]
      public void Put(int id, [FromBody] string value)
      {
      }

      // DELETE api/<BinController>/5
      [HttpDelete("{id}")]
      public void Delete(int id)
      {
      }
   }
}
