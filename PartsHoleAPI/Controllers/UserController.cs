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
   private readonly IUserCollection _userService;
   private readonly ICollectionService<IPartModel> _partsCollection;
   private readonly IInvoiceService _invoiceService;
   private readonly ILogger<UserController> _logger;

   public UserController(
      ILogger<UserController> logger,
      IUserCollection userCollection,
      ICollectionService<IPartModel> partsCollection,
      IInvoiceService invoiceService)
   {
      _userService = userCollection;
      _partsCollection = partsCollection;
      _invoiceService = invoiceService;
      _logger = logger;
   }

   #region API Methods
   /// <summary>
   /// Gets an <see cref="IUserModel"/> based on the given <see cref="ObjectId"/>.
   /// <list type="table">
   ///   <listheader>
   ///      <term>Method</term>
   ///      <description>URL</description>
   ///   </listheader>
   ///   <item>
   ///      <term>GET</term>
   ///      <description>api/user/{<paramref name="id"/>}</description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="id">The <see cref="ObjectId"/> of the <see cref="IUserModel"/>.</param>
   /// <returns><see cref="IUserModel"/> if found. Otherwise null.</returns>
   [HttpGet("{id:length(24)}")]
   public async Task<ActionResult<IUserModel>> Get(string id)
   {
      if (string.IsNullOrEmpty(id))
      {
         StatusCode(StatusCodes.Status400BadRequest);
         return BadRequest(id);
      }
      var user = await _userService.GetFromDatabaseAsync(id);
      if (user is null)
         return NotFound(id);
      return Ok(user);
   }

   /// <summary>
   /// Gets the users data from the <see cref="IUserModel"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/user/data</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="IUserModel"/> <paramref name="user"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="user"><see cref="IUserModel"/> to get data for.</param>
   /// <returns><see cref="IUserData"/> containing all the <paramref name="user"/> data.</returns>
   [HttpPost("data")]
   public async Task<ActionResult<APIResponse<IUserData?>>> PostGetUserData([FromBody] UserModel user)
   {
      if (user is null)
      {
         _logger.LogWarning("Unable to construct user model from body.");
         return BadRequest(new APIResponse<IUserData?>("POST", "No user data found in body."));
      }
      if (user.Parts is null && user.Invoices is null)
      {
         _logger.LogWarning("No parts or invoice data found.");
         return BadRequest(new APIResponse<IUserData?>("POST", "User has no data."));
      }
      var response = await _userService.GetUserDataFromDatabaseAsync(user);
      return response is null
         ? NotFound(new APIResponse<IUserData?>("POST", "No user data found Found"))
         : Ok(new APIResponse<IUserData?>(response, "POST"));
   }

   // POST api/User
   /// <summary>
   /// Creates a new <see cref="IUserModel"/> and saves it to the database.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/user</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="IUserModel"/> <paramref name="newUser"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="newUser"><see cref="IUserModel"/> to create.</param>
   /// <returns>True if successful, otherwise False.</returns>
   [HttpPost]
   public async Task<ActionResult<APIResponse<bool>>> Post([FromBody] UserModel? newUser)
   {
      if (newUser is null)
      {
         _logger.LogWarning("Unable to construct user model from body.");
         return BadRequest(new APIResponse<bool>(false, "POST", "Unable to construct user model from body."));
      }
      if (string.IsNullOrEmpty(newUser._id))
      {
         _logger.LogWarning("User model has no valid ID.");
         return BadRequest(new APIResponse<bool>(false, "POST", "User model has no valid ID."));
      }
      return Ok(new APIResponse<bool>(await _userService.AddToDatabaseAsync(newUser), "POST"));
   }

   /// <summary>
   /// Adds a newly created <see cref="IPartModel"/> to the <see cref="IUserModel"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/user/add-part</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="AppendRequestModel"/> <paramref name="data"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="data"><see cref="IUserModel"/> ID and <see cref="IPartModel"/> ID.</param>
   /// <returns>True if successful, otherwise False.</returns>
   [HttpPost("add-part")]
   public async Task<ActionResult<APIResponse<bool>>> PostAppendPart([FromBody] AppendRequestModel data)
   {
      if (data.UserId is null)
         return BadRequest(new APIResponse<bool>(false, "POST", "Unable to find user ID."));
      if (string.IsNullOrEmpty(data.ModelId))
         return BadRequest(new APIResponse<bool>(false, "POST", "Part ID not found."));
      if (data.ModelId.Length != 24)
         return BadRequest(new APIResponse<bool>(false, "POST", "Part ID is not valid."));
      var user = await _userService.GetFromDatabaseAsync(data.UserId);
      if (user is null)
         return NotFound("User not found.");
      var part = await _partsCollection.GetFromDatabaseAsync(data.ModelId);
      if (part is null)
         return NotFound("Part not found.");
      user.Parts.Add(data.ModelId);
      return new APIResponse<bool>(await _userService.UpdateDatabaseAsync(data.UserId, user), "POST");
   }

   /// <summary>
   /// Adds a newly created <see cref="IInvoiceModel"/> to the <see cref="IUserModel"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/user/add-invoice</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="AppendRequestModel"/> <paramref name="data"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="data"><see cref="IUserModel"/> ID and <see cref="IInvoiceModel"/> ID.</param>
   /// <returns>True if successful, otherwise False.</returns>
   [HttpPost("add-invoice")]
   public async Task<ActionResult<APIResponse<bool>>> PostAppendInvoice([FromBody] AppendRequestModel data)
   {
      if (data.UserId is null)
         return BadRequest(new APIResponse<bool>(false, "POST", "Unable to find user ID."));
      if (string.IsNullOrEmpty(data.ModelId))
         return BadRequest(new APIResponse<bool>(false, "POST", "Part ID not found."));
      if (data.ModelId.Length != 24)
         return BadRequest(new APIResponse<bool>(false, "POST", "Part ID is not valid."));
      var user = await _userService.GetFromDatabaseAsync(data.UserId);
      if (user is null)
         return NotFound("User not found.");
      var invoice = await _invoiceService.GetFromDatabaseAsync(data.ModelId);
      if (invoice is null)
         return NotFound("Invoice not found.");
      user.Invoices.Add(data.ModelId);
      return new APIResponse<bool>(await _userService.UpdateDatabaseAsync(data.UserId, user), "POST");
   }

   /// <summary>
   /// Updates an <see cref="IUserModel"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>PUT</term>
   ///      <description>api/user</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="IUserModel"/> <paramref name="updatedUser"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="updatedUser">Udated <see cref="IUserModel"/>.</param>
   /// <returns>True if successful, otherwise False.</returns>
   [HttpPut]
   public async Task<ActionResult<APIResponse<bool>>> Put([FromBody] UserModel updatedUser) =>
      updatedUser is null
         ? BadRequest(new APIResponse<bool>(false, "PUT", "Unable to find user."))
         : Ok(new APIResponse<bool>(await _userService.UpdateDatabaseAsync(updatedUser._id, updatedUser), "PUT"));

   /// <summary>
   /// Deletes an <see cref="IUserModel"/> based on the <see cref="ObjectId"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>DELETE</term>
   ///      <description>api/user/{<paramref name="id"/>}</description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="updatedUser">Udated <see cref="IUserModel"/>.</param>
   /// <returns>True if successful, otherwise False.</returns>
   [HttpDelete("{id:length(24)}")]
   public async Task<ActionResult<APIResponse<bool>>> Delete(string id) =>
      string.IsNullOrEmpty(id)
         ? BadRequest(new APIResponse<bool>(false, "DELETE", "Unable to find user ID."))
         : await _userService.DeleteFromDatabaseAsync(id)
            ? Ok(new APIResponse<bool>(true, "DELETE", $"User {id} Deleted"))
            : BadRequest(new APIResponse<bool>(false, "DELETE", "Unable to delete user."));
   #endregion
}
