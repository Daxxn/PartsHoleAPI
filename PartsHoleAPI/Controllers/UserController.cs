using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;

using PartsHoleAPI.DBServices;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

using PartsHoleRestLibrary.Enums;
using PartsHoleRestLibrary.Exceptions;
using PartsHoleRestLibrary.Requests;
using PartsHoleRestLibrary.Responses;

namespace PartsHoleAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
   private readonly IUserService _userService;
   private readonly ICollectionService<IPartModel> _partsService;
   private readonly ICollectionService<IBinModel> _binService;
   private readonly IInvoiceService _invoiceService;
   private readonly ILogger<UserController> _logger;

   public UserController(
      ILogger<UserController> logger,
      IUserService userService,
      ICollectionService<IPartModel> partsService,
      ICollectionService<IBinModel> binService,
      IInvoiceService invoiceService)
   {
      _userService = userService;
      _partsService = partsService;
      _binService = binService;
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
   /// Adds the passed model <see cref="ObjectId"/> to the users id collection.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/user/add-model</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="RequestUpdateListModel"/> <paramref name="requestData"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="requestData"><see cref="IUserModel"/> ID, <see cref="IBinModel"/> ID, and <see cref="ModelIDSelector"/>.</param>
   /// <returns>True if successful, otherwise False.</returns>
   [HttpPost("add-model")]
   public async Task<ActionResult<APIResponse<bool>>> PostAppendModel([FromBody] RequestUpdateListModel requestData)
   {
      try
      {
         if (string.IsNullOrEmpty(requestData.UserId))
            return BadRequest(new APIResponse<bool>(false, "POST", "User ID was not provided."));
         if (string.IsNullOrEmpty(requestData.ModelId))
            return BadRequest(new APIResponse<bool>(false, "POST", "Model ID was not provided."));
         var modelId = (ModelIDSelector?)requestData.PropId ?? ModelIDSelector.NONE;
         if (modelId == ModelIDSelector.NONE)
            return BadRequest(new APIResponse<bool>(false, "POST", "Property type does not match."));
         var result = await _userService.AppendModelToUserAsync(requestData.UserId, requestData.ModelId, modelId);
         if (result)
         {
            return Ok(new APIResponse<bool>(true, "POST"));
         }
         return BadRequest(new APIResponse<bool>(false, "POST", "Failed to remove model."));
      }
      catch (ModelNotFoundException e)
      {
         return NotFound(new APIResponse<bool>(false, "POST", $"Unable to find {e.ModelName}"));
      }
      catch (Exception)
      {
         throw;
      }
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
            ? Ok(new APIResponse<bool>(true, "DELETE", $"User {id} Deleted."))
            : BadRequest(new APIResponse<bool>(false, "DELETE", "Unable to delete user."));

   /// <summary>
   /// Removes a model <see cref="ObjectId"/> from the <see cref="IUserModel"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>DELETE</term>
   ///      <description>api/user/{<paramref name="id"/>}</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="RequestUpdateListModel"/> <paramref name="requestData"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="requestData">Contains the User <see cref="ObjectId"/>, Model <see cref="ObjectId"/>, and the Property <see cref="ModelIDSelector"/>.</param>
   /// <returns>True if successful, otherwise False.</returns>
   [HttpDelete("remove-model")]
   public async Task<ActionResult<APIResponse<bool>>> DeleteRemoveModel([FromBody] RequestUpdateListModel requestData)
   {
      if (string.IsNullOrEmpty(requestData.UserId))
         return BadRequest(new APIResponse<bool>(false, "DELETE", "User ID was not provided."));
      if (string.IsNullOrEmpty(requestData.ModelId))
         return BadRequest(new APIResponse<bool>(false, "DELETE", "Model ID was not provided."));
      var modelId = (ModelIDSelector?)requestData.PropId ?? ModelIDSelector.NONE;
      if (modelId == ModelIDSelector.NONE)
         return BadRequest(new APIResponse<bool>(false, "DELETE", "Property type does not match."));
      var result = await _userService.RemoveModelFromUserAsync(requestData.UserId, requestData.ModelId, modelId);
      if (result)
      {
         return Ok(new APIResponse<bool>(true, "DELETE"));
      }
      return BadRequest(new APIResponse<bool>(false, "DELETE", "Failed to remove model."));
   }
   #endregion
}
