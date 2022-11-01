using Microsoft.AspNetCore.Mvc;

using PartsHoleAPI.DBServices;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class UserController : ControllerBase
   {
      private readonly UserCollection _userCollection;
      private readonly ILogger<UserController> _logger;

      public UserController(ILogger<UserController> logger, UserCollection userCollection)
      {
         _userCollection = userCollection;
         _logger = logger;
      }

      // GET: api/<UserController>
      [HttpGet]
      public ActionResult<string> Get()
      {
         _logger.Log(LogLevel.Debug, "Attempt to call generic GET method.");
         return Ok("Not allowed. A User ID is required.");
      }

      // GET api/<UserController>/5
      [HttpGet("{id:length(24)}")]
      public async Task<ActionResult<UserModel>> Get(string id)
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
      public void Post([FromBody] UserModel? value)
      {
         if (value is null)
         {
            _logger.LogWarning("Unable to construct user model from body.");
            return;
         }

      }

      // POST api/<UserController>/login
      [HttpPost("login")]
      public void PostLogin([FromBody] string value)
      {

      }

      // PUT api/<UserController>/test
      [HttpPut("{id:length(24)}")]
      public void Put(string id, [FromBody] UserModel? value)
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
         if (await _userCollection.DeleteFromDatabaseAsync(id))
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
