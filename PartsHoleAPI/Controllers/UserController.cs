using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

using PartsHoleAPI.DBServices;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class UserController : ControllerBase
   {
      private readonly IUserCollection _userCollection;
      private readonly ILogger<UserController> _logger;

      public UserController(ILogger<UserController> logger, IUserCollection userCollection)
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

      // GET api/<UserController>/{id}
      [HttpGet("{id:length(24)}")]
      public async Task<ActionResult<IUserModel>> Get(string id)
      {
         if (string.IsNullOrEmpty(id))
         {
            StatusCode(StatusCodes.Status400BadRequest);
            return BadRequest(id);
         }
         var user = await _userCollection.GetFromDatabaseAsync(id);
         if (user is null) return NotFound(id);
         return Ok(user);
      }

      // POST api/<UserController>/data
      [HttpPost("data")]
      public async Task<ActionResult<IUserData>> PostGetUserData([FromBody] UserModel user)
      {
         if (user is null)
         {
            _logger.LogWarning("Unable to construct user model from body.");
            return BadRequest("No user data found in body.");
         }
         if (user.Parts is null && user.Invoices is null)
         {
            _logger.LogWarning("No parts or invoice data found.");
            return BadRequest("User has no data.");
         }
         var response = await _userCollection.GetUserDataFromDatabaseAsync(user);
         return response is null
            ? NotFound("No user data found Found")
            : Ok(response);
      }

      // POST api/<UserController>
      [HttpPost]
      public async Task<ActionResult<bool>> Post([FromBody] UserModel? value)
      {
         if (value is null)
         {
            _logger.LogWarning("Unable to construct user model from body.");
            return BadRequest(false);
         }
         if (string.IsNullOrEmpty(value.Id))
         {
            _logger.LogWarning("User model has no valid ID.");
            return BadRequest(false);
         }
         return Ok(await _userCollection.AddToDatabaseAsync(value));
      }

      // PUT api/<UserController>/{id}
      [HttpPut("{id:length(24)}")]
      public async Task<ActionResult<bool>> Put(string id, [FromBody] UserModel? value) =>
         value is null || string.IsNullOrEmpty(id)
            ? (ActionResult<bool>)BadRequest(false)
            : (ActionResult<bool>)Ok(await _userCollection.UpdateDatabaseAsync(id, value));

      // DELETE api/<UserController>/{id}
      [HttpDelete("{id:length(24)}")]
      public async Task<ActionResult<bool>> Delete(string id) => 
         string.IsNullOrEmpty(id)
            ? (ActionResult<bool>)BadRequest(false)
            : await _userCollection.DeleteFromDatabaseAsync(id) ? Ok(true) : BadRequest(false);
   }
}
