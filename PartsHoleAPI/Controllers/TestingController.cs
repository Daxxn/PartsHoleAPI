using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using PartsHoleAPI.Collections;
using PartsHoleLib.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PartsHoleAPI.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class TestingController : ControllerBase
   {
      private readonly PartsCollection _partsCollection;
      public TestingController(PartsCollection partsCollection)
      {
         _partsCollection = partsCollection;
      }

      // GET: api/<TestingController>
      [HttpGet]
      public async Task<ActionResult<IPartModel>> Get()
      {
         var part = await _partsCollection.GetFromDatabaseAsync("6360180d1a792e2787223cff");

         if (part is null)
         {
            StatusCode(StatusCodes.Status404NotFound);
            return NotFound();
         }
         StatusCode(StatusCodes.Status200OK);
         return Ok(part);
      }

      // GET api/<TestingController>/5
      [HttpGet("{id:length(24)}")]
      public async Task<ActionResult<IPartModel>> Get(string id)
      {
         if (string.IsNullOrEmpty(id))
         {
            StatusCode(StatusCodes.Status400BadRequest);
            return null;
         }
         var part = await _partsCollection.GetFromDatabaseAsync(id);
         if (part is null)
            return NotFound(id);
         return Ok(part);
      }

      // POST api/<TestingController>
      [HttpPost]
      public async Task Post([FromBody] string value)
      {
         StatusCode(StatusCodes.Status501NotImplemented);
      }

      // PUT api/<TestingController>/5
      [HttpPut("{id}")]
      public async Task Put(int id, [FromBody] string value)
      {
         StatusCode(StatusCodes.Status501NotImplemented);
      }

      // DELETE api/<TestingController>/5
      [HttpDelete("{id}")]
      public async Task<int> Delete(int id)
      {
         StatusCode(StatusCodes.Status501NotImplemented);
         return 0;
      }
   }
}
