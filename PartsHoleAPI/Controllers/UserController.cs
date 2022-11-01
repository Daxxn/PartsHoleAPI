using Microsoft.AspNetCore.Mvc;

using PartsHoleAPI.Collections;
using PartsHoleAPI.Models.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PartsHoleAPI.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class UserController : ControllerBase
   {
      private readonly UserCollection _userCollection;

      public UserController(UserCollection userCollection)
      {
         _userCollection = userCollection;
      }

      // GET: api/<UserController>
      [HttpGet]
      public string Get()
      {
         StatusCode(StatusCodes.Status400BadRequest);
         return "Not Allowed. A user ID is required.";
      }

      // GET api/<UserController>/5
      [HttpGet("{id:length(24)}")]
      public async Task<ActionResult<IUserModel>> Get(string id)
      {
         if (string.IsNullOrEmpty(id))
         {
            StatusCode(StatusCodes.Status400BadRequest);
            return BadRequest(id);
         }
         await Response.WriteAsJsonAsync(await _userCollection.GetFromDatabaseAsync(id));
         var user = await _userCollection.GetFromDatabaseAsync(id);
         if (user is null)
            return NotFound(id);
         return Ok(user);
      }

      // POST api/<UserController>
      [HttpPost]
      public void Post([FromBody] IUserModel? value)
      {
         if (value is null) return;
      }

      // POST api/<UserController>/login
      [HttpPost("login")]
      public void PostLogin([FromBody] string value)
      {

      }

      // PUT api/<UserController>/test
      [HttpPut("{id:length(24)}")]
      public void Put(string id, [FromBody] IUserModel? value)
      {
      }

      // DELETE api/<UserController>/5
      [HttpDelete("{id:length(24)}")]
      public async Task Delete(string id)
      {
         if (string.IsNullOrEmpty(id))
         {
            StatusCode(StatusCodes.Status400BadRequest);
            return;
         }
         if (await _userCollection.DeleteFromDatabase(id))
         {
            StatusCode(StatusCodes.Status202Accepted);
         }
         else
         {
            StatusCode(StatusCodes.Status400BadRequest);
         }
      }
   }
}
