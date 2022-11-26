using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;

using PartsHoleAPI.DBServices.Interfaces;
using PartsHoleAPI.Utils;

using PartsHoleLib;
using PartsHoleLib.Enums;

using PartsHoleRestLibrary.Exceptions;
using PartsHoleRestLibrary.Requests;
using PartsHoleRestLibrary.Responses;

namespace PartsHoleAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
   private readonly IUserService _userService;
   private readonly ICollectionService<PartModel> _partsService;
   private readonly ICollectionService<BinModel> _binService;
   private readonly IInvoiceService _invoiceService;
   private readonly ILogger<UserController> _logger;

   public UserController(
      ILogger<UserController> logger,
      IUserService userService,
      ICollectionService<PartModel> partsService,
      ICollectionService<BinModel> binService,
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
   /// Gets an <see cref="UserModel"/> based on the given <see cref="ObjectId"/>.
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
   /// <param name="id">The <see cref="ObjectId"/> of the <see cref="UserModel"/>.</param>
   /// <returns><see cref="UserModel"/> if found. Otherwise null.</returns>
   [HttpGet("{id:length(24)}")]
   public async Task<APIResponse<UserModel>> Get(string id)
   {
      var user = await _userService.GetFromDatabaseAsync(id);
      if (user != null)
      {
         _logger.ApiLogInfo("GET", "api/user/{id}", $"User {id} found.");
         return new(user, "GET");
      }
      return new("GET", "Unable to find user.");
   }

   /// <summary>
   /// Gets the users data from the <see cref="UserModel"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/user/data</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="UserModel"/> <paramref name="user"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="user"><see cref="UserModel"/> to get data for.</param>
   /// <returns><see cref="UserData"/> containing all the <paramref name="user"/> data.</returns>
   [HttpPost("data")]
   public async Task<APIResponse<UserData?>> PostGetUserData([FromBody] UserModel user)
   {
      if (user is null)
      {
         _logger.ApiLogWarn("POST", "api/user/data", "Unable to construct user model from body.");
         return new("POST", "No user data found in body.");
      }
      if (user.Parts is null && user.Invoices is null && user.Bins is null && user.PartNumbers is null)
      {
         _logger.ApiLogWarn("POST", "api/user/data", "No parts or invoice data found.");
         return new("POST", "User has no data.");
      }
      var response = await _userService.GetUserDataFromDatabaseAsync(user);
      return response is null
         ? new("POST", "No user data found Found")
         : new(response, "POST");
   }

   /// <summary>
   /// Creates a new <see cref="UserModel"/> and saves it to the database.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/user</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="UserModel"/> <paramref name="newUser"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="newUser"><see cref="UserModel"/> to create.</param>
   /// <returns>True if successful, otherwise False.</returns>
   [HttpPost]
   public async Task<ActionResult<APIResponse<bool>>> Post([FromBody] UserModel? newUser)
   {
      if (newUser is null)
      {
         _logger.ApiLogWarn("POST", "api/user", "Unable to construct user model from body.");
         return BadRequest(new APIResponse<bool>(false, "POST", "Unable to construct user model from body."));
      }
      if (string.IsNullOrEmpty(newUser._id))
      {
         _logger.ApiLogWarn("POST", "api/user", "User model has no valid ID.");
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
   /// <param name="requestData"><see cref="UserModel"/> ID, <see cref="BinModel"/> ID, and <see cref="ModelIDSelector"/>.</param>
   /// <returns>True if successful, otherwise False.</returns>
   [HttpPost("add-model")]
   public async Task<ActionResult<APIResponse<bool>>> PostAppendModel([FromBody] RequestUpdateListModel requestData)
   {
      try
      {
         if (string.IsNullOrEmpty(requestData.UserId))
         {
            _logger.ApiLogInfo("POST", "api/user/add-model", "User ID was not provided.");
            return BadRequest(new APIResponse<bool>(false, "POST", "User ID was not provided."));
         }
         if (string.IsNullOrEmpty(requestData.ModelId))
         {
            _logger.ApiLogInfo("POST", "api/user/add-model", "Model ID was not provided.");
            return BadRequest(new APIResponse<bool>(false, "POST", "Model ID was not provided."));
         }
         var modelId = (ModelIDSelector?)requestData.PropId ?? ModelIDSelector.NONE;
         if (modelId == ModelIDSelector.NONE)
         {
            _logger.ApiLogWarn("POST", "api/user/add-model", "Property type does not match.");
            return BadRequest(new APIResponse<bool>(false, "POST", "Property type does not match."));
         }
         var result = await _userService.AppendModelToUserAsync(requestData.UserId, requestData.ModelId, modelId);
         if (result)
         {
            _logger.ApiLogInfo("POST", "api/user/add-model", "Success");
            return Ok(new APIResponse<bool>(true, "POST"));
         }
         _logger.ApiLogWarn("POST", "api/user/add-model", "Failed to remove model.");
         return BadRequest(new APIResponse<bool>(false, "POST", "Failed to remove model."));
      }
      catch (ModelNotFoundException e)
      {
         _logger.ApiLogWarn("POST", "api/user/add-model", $"Unable to find {e.ModelName}");
         return NotFound(new APIResponse<bool>(false, "POST", $"Unable to find {e.ModelName}"));
      }
      catch (Exception e)
      {
         _logger.ApiLogError("POST", "api/user/add-model", "Internal Error", e);
         throw;
      }
   }

   /// <summary>
   /// Updates an <see cref="UserModel"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>PUT</term>
   ///      <description>api/user</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="UserModel"/> <paramref name="updatedUser"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="updatedUser">Udated <see cref="UserModel"/>.</param>
   /// <returns>True if successful, otherwise False.</returns>
   [HttpPut]
   public async Task<ActionResult<APIResponse<bool>>> Put([FromBody] UserModel updatedUser)
   {
      if (updatedUser == null)
      {
         _logger.ApiLogWarn("PUT", "api/user", "Unable to find user.");
         return BadRequest(new APIResponse<bool>(false, "PUT", "Unable to find user."));
      }

      try
      {
         if (await _userService.UpdateDatabaseAsync(updatedUser._id, updatedUser))
         {
            _logger.ApiLogInfo("PUT", "api/user", $"Successfully updated user {updatedUser._id}");
            return Ok(new APIResponse<bool>(true, "PUT"));
         }
         _logger.ApiLogWarn("PUT", "api/user", $"Unable to update user {updatedUser._id}");
         return BadRequest(new APIResponse<bool>(false, "PUT"));
      }
      catch (Exception e)
      {
         _logger.ApiLogError("PUT", "api/user", "Internal Error", e);
         throw;
      }
   }

   /// <summary>
   /// Deletes an <see cref="UserModel"/> based on the <see cref="ObjectId"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>DELETE</term>
   ///      <description>api/user/{<paramref name="id"/>}</description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="id"><see cref="ObjectId"/> of the <see cref="UserModel"/> to delete.</param>
   /// <returns>True if successful, otherwise False.</returns>
   [HttpDelete("{id:length(24)}")]
   public async Task<ActionResult<APIResponse<bool>>> Delete(string id)
   {
      try
      {
         if (string.IsNullOrEmpty(id))
         {
            _logger.ApiLogWarn("DELETE", "api/user/{id}", "Provided id was null.");
            return BadRequest(new APIResponse<bool>(false, "DELETE", "Unable to find user ID."));
         }
         if (await _userService.DeleteFromDatabaseAsync(id))
         {
            _logger.ApiLogInfo("DELETE", "api/user/{id}", $"Sucessfully deleted user {id}");
            return Ok(new APIResponse<bool>(true, "DELETE", $"User {id} Deleted."));
         }
         _logger.ApiLogWarn("DELETE", "api/user/{id}", $"Unable to deleted user {id}");
         return BadRequest(new APIResponse<bool>(false, "DELETE", "Unable to delete user."));
      }
      catch (Exception e)
      {
         _logger.ApiLogError("DELETE", "api/user/{id}", "Internal Error", e);
         throw;
      }
   }

   /// <summary>
   /// Removes a model <see cref="ObjectId"/> from the <see cref="UserModel"/>.
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
      try
      {
         if (string.IsNullOrEmpty(requestData.UserId))
         {
            _logger.ApiLogWarn("DELETE", "api/user/{id}", "User ID was null.");
            return BadRequest(new APIResponse<bool>(false, "DELETE", "User ID was not provided."));
         }
         if (string.IsNullOrEmpty(requestData.ModelId))
         {
            _logger.ApiLogWarn("DELETE", "api/user/{id}", "Model ID was null.");
            return BadRequest(new APIResponse<bool>(false, "DELETE", "Model ID was not provided."));
         }
         var modelId = (ModelIDSelector?)requestData.PropId ?? ModelIDSelector.NONE;
         if (modelId == ModelIDSelector.NONE)
         {
            _logger.ApiLogWarn("DELETE", "api/user/{id}", "Property type does not match.");
            return BadRequest(new APIResponse<bool>(false, "DELETE", "Property type does not match."));
         }
         var result = await _userService.RemoveModelFromUserAsync(requestData.UserId, requestData.ModelId, modelId);
         if (result)
         {
            _logger.ApiLogInfo("DELETE", "api/user/{id}", $"Successfuly removed model {requestData.ModelId} from user {requestData.UserId}.");
            return Ok(new APIResponse<bool>(true, "DELETE"));
         }
         _logger.ApiLogWarn("DELETE", "api/user/{id}", $"Failed to remove model {requestData.ModelId} from user {requestData.UserId}.");
         return BadRequest(new APIResponse<bool>(false, "DELETE", "Failed to remove model."));
      }
      catch (Exception e)
      {
         _logger.ApiLogError("DELETE", "api/user/{id}", "Internal Error", e);
         throw;
      }
   }
   #endregion
}
