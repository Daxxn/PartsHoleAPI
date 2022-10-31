using Microsoft.AspNetCore.Mvc;

using PartsHoleAPI.Models.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PartsHoleAPI.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class UserController : ControllerBase
   {
      public UserController(ILogger<IUserModel> logger)
      {

      }
      // GET: api/<UserController>
      [HttpGet]
      public IEnumerable<string> Get()
      {
         return new string[] { "value1", "value2" };
      }

      // GET api/<UserController>/5
      [HttpGet("{id}")]
      public string Get(int id)
      {
         return "value";
      }

      // POST api/<UserController>
      [HttpPost]
      public void Post([FromBody] string value)
      {
      }

      // PUT api/<UserController>/5
      [HttpPut("{id}")]
      public void Put(int id, [FromBody] string value)
      {
      }

      // DELETE api/<UserController>/5
      [HttpDelete("{id}")]
      public void Delete(int id)
      {
      }
   }
}
