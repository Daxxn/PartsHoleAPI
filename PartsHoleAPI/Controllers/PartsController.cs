using Microsoft.AspNetCore.Mvc;

using PartsHoleAPI.Collections;
using PartsHoleAPI.Models.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PartsHoleAPI.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class PartsController : ControllerBase
   {
      private readonly PartsCollection _partsCollection;

      public PartsController(PartsCollection partsCollection)
      {
         _partsCollection = partsCollection;
      }

      // GET: api/<PartsController>
      [HttpGet]
      public string Get()
      {
         NoContent();
         return "Not allowed. A part ID is required.";
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
      public async Task Post([FromBody] string value)
      {

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
