using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

using PartsHoleAPI.DBServices;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

using PartsHoleRestLibrary.Requests;
using PartsHoleRestLibrary.Responses;

namespace PartsHoleAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
   private readonly IUserCollection _collection;
   private readonly ICollectionService<IPartModel> _partsCollection;
   private readonly ICollectionService<IInvoiceModel> _invoiceCollection;
   private readonly ILogger<UserController> _logger;

   public UserController(
      ILogger<UserController> logger,
      IUserCollection userCollection,
      ICollectionService<IPartModel> partsCollection,
      ICollectionService<IInvoiceModel> invoiceCollection)
   {
      _collection = userCollection;
      _partsCollection = partsCollection;
      _invoiceCollection = invoiceCollection;
      _logger = logger;
   }

   // GET: api/User
   [HttpGet]
   public ActionResult<string> Get()
   {
      _logger.Log(LogLevel.Debug, "Attempt to call generic GET method.");
      return Ok("Not allowed. A User ID is required.");
   }

   #region API Methods
   // GET api/User/{id}
   [HttpGet("{id:length(24)}")]
   public async Task<ActionResult<IUserModel>> Get(string id)
   {
      if (string.IsNullOrEmpty(id))
      {
         StatusCode(StatusCodes.Status400BadRequest);
         return BadRequest(id);
      }
      var user = await _collection.GetFromDatabaseAsync(id);
      if (user is null)
         return NotFound(id);
      return Ok(user);
   }

   // POST api/User/data
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
      var response = await _collection.GetUserDataFromDatabaseAsync(user);
      return response is null
         ? NotFound("No user data found Found")
         : Ok(response);
   }

   // POST api/User
   [HttpPost]
   public async Task<ActionResult<APIResponse<bool>>> Post([FromBody] UserModel? value)
   {
      if (value is null)
      {
         _logger.LogWarning("Unable to construct user model from body.");
         return BadRequest(new APIResponse<bool>(false, "POST", "Unable to construct user model from body."));
      }
      if (string.IsNullOrEmpty(value.Id))
      {
         _logger.LogWarning("User model has no valid ID.");
         return BadRequest(new APIResponse<bool>(false, "POST", "User model has no valid ID."));
      }
      return Ok(new APIResponse<bool>(await _collection.AddToDatabaseAsync(value), "POST"));
   }

   // POST api/User/add-part/{id}
   // Body : ObjectId partId
   [HttpPost("add-part")]
   public async Task<ActionResult<APIResponse<bool>>> PostAppendPart([FromBody] AppendRequestModel data)
   {
      if (data.UserId is null)
         return BadRequest(new APIResponse<bool>(false, "POST", "Unable to find user ID."));
      if (string.IsNullOrEmpty(data.ModelId))
         return BadRequest(new APIResponse<bool>(false, "POST", "Part ID not found."));
      if (data.ModelId.Length != 24)
         return BadRequest(new APIResponse<bool>(false, "POST", "Part ID is not valid."));
      var user = await _collection.GetFromDatabaseAsync(data.UserId);
      if (user is null)
         return NotFound("User not found.");
      var part = await _partsCollection.GetFromDatabaseAsync(data.ModelId);
      if (part is null)
         return NotFound("Part not found.");
      user.Parts.Add(data.ModelId);
      return new APIResponse<bool>(await _collection.UpdateDatabaseAsync(data.UserId, user), "POST");
   }

   [HttpPost("add-invoice")]
   public async Task<ActionResult<APIResponse<bool>>> PostAppendInvoice([FromBody] AppendRequestModel data)
   {
      if (data.UserId is null)
         return BadRequest(new APIResponse<bool>(false, "POST", "Unable to find user ID."));
      if (string.IsNullOrEmpty(data.ModelId))
         return BadRequest(new APIResponse<bool>(false, "POST", "Part ID not found."));
      if (data.ModelId.Length != 24)
         return BadRequest(new APIResponse<bool>(false, "POST", "Part ID is not valid."));
      var user = await _collection.GetFromDatabaseAsync(data.ModelId);
      if (user is null)
         return NotFound("User not found.");
      var invoice = await _invoiceCollection.GetFromDatabaseAsync(data.ModelId);
      if (invoice is null)
         return NotFound("Invoice not found.");
      user.Invoices.Add(data.ModelId);
      return new APIResponse<bool>(await _collection.UpdateDatabaseAsync(data.UserId, user), "POST");
   }

   // PUT api/User/{id}
   [HttpPut("{id:length(24)}")]
   public async Task<ActionResult<APIResponse<bool>>> Put(string id, [FromBody] UserModel value) =>
      value is null || string.IsNullOrEmpty(id)
         ? BadRequest(new APIResponse<bool>(false, "PUT", "Unable to find user ID."))
         : Ok(new APIResponse<bool>(await _collection.UpdateDatabaseAsync(id, value), "PUT"));

   // DELETE api/User/{id}
   [HttpDelete("{id:length(24)}")]
   public async Task<ActionResult<APIResponse<bool>>> Delete(string id) =>
      string.IsNullOrEmpty(id)
         ? BadRequest(new APIResponse<bool>(false, "DELETE", "Unable to find user ID."))
         : await _collection.DeleteFromDatabaseAsync(id)
            ? Ok(new APIResponse<bool>(true, "DELETE", $"User {id} Deleted"))
            : BadRequest(new APIResponse<bool>(false, "DELETE", "Unable to delete user."));
   #endregion
}
