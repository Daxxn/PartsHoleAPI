using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PartsHoleAPI.DBServices.Interfaces;
using PartsHoleLib;
using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestingController : ControllerBase
{
   private readonly ICollectionService<PartModel> _partsCollection;
   public TestingController(ICollectionService<PartModel> partsCollection)
   {
      _partsCollection = partsCollection;
   }

   // GET: api/<TestingController>
   [HttpGet]
   public async Task<ActionResult<PartModel?>> Get()
   {
      var part = await _partsCollection.GetFromDatabaseAsync("6360180d1a792e2787223cff");

      return part is null ? NotFound() : Ok(part);
   }

   // GET api/<TestingController>/6360180d1a792e2787223cff
   [HttpGet("{id:length(24)}")]
   public async Task<ActionResult<PartModel?>> Get(string id)
   {
      if (string.IsNullOrEmpty(id))
      {
         return BadRequest();
      }
      var part = await _partsCollection.GetFromDatabaseAsync(id);
      return part is null ? NotFound() : Ok(part);
   }

   // POST api/<TestingController>
   [HttpPost]
   public async Task Post([FromBody] string value)
   {
      StatusCode(StatusCodes.Status501NotImplemented);
   }

   // PUT api/<TestingController>/6360180d1a792e2787223cff
   [HttpPut("{id}")]
   public async Task Put(int id, [FromBody] string value)
   {
      StatusCode(StatusCodes.Status501NotImplemented);
   }

   // DELETE api/<TestingController>/6360180d1a792e2787223cff
   [HttpDelete("{id}")]
   public async Task<int> Delete(int id)
   {
      StatusCode(StatusCodes.Status501NotImplemented);
      return 0;
   }
}
