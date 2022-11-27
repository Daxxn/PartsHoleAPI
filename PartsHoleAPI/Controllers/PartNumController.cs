using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;

using PartsHoleAPI.DBServices.Interfaces;
using PartsHoleAPI.Utils;

using PartsHoleLib;

using PartsHoleRestLibrary.Exceptions;
using PartsHoleRestLibrary.Requests;
using PartsHoleRestLibrary.Responses;

namespace PartsHoleAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PartNumController : ControllerBase
{
   #region Props
   private readonly ILogger<PartNumController> _logger;
   private readonly IPartNumberService _partNumberService;
   #endregion
   public PartNumController(
      IPartNumberService partNumberService,
      ILogger<PartNumController> logger
      )
   {
      _logger = logger;
      _partNumberService = partNumberService;
   }

   /// <summary>
   /// Get a <see cref="PartNumber"/> from the database.
   /// <list type="table">
   ///   <item>
   ///      <term>GET</term>
   ///      <description>api/partnums/{<paramref name="id"/>}</description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="id"><see cref="ObjectId"/> to find.</param>
   /// <returns>The <see cref="PartNumber"/> if found. Otherwise <see langword="null"/>.</returns>
   [HttpGet("{id:length(24)}")]
   public async Task<APIResponse<PartNumber>> Get(string id)
   {
      try
      {
         var foundPN = await _partNumberService.GetFromDatabaseAsync(id);
         if (foundPN is null)
         {
            _logger.ApiLogInfo("GET", "api/partnums", "Part Number not found.");
            return new("GET", "Part Number not found.");
         }
         _logger.ApiLogDebug("GET", "api/partnums", $"Success : {id}");
         return new(foundPN, "GET");
      }
      catch (Exception e)
      {
         _logger.ApiLogError("GET", "api/partnums", "Error", e);
         return new("GET", e.Message);
      }
   }

   /// <summary>
   /// Generate New part number based on the provided.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/partnums/gen</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="PartNumberRequestModel"/> <paramref name="requestData"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="requestData">Type and SubType request data.</param>
   [HttpPost("gen")]
   public async Task<APIResponse<PartNumber>> PostGeneratePartNumber([FromBody] PartNumberRequestModel requestData)
   {
      try
      {
         if (string.IsNullOrEmpty(requestData.UserId))
         {
            _logger.ApiLogInfo("POST", "api/partnums/gen", "User ID not provided.");
            return new APIResponse<PartNumber>("POST", "User ID not provided.");
         }
         if (requestData.FullCategory is 0)
         {
            _logger.ApiLogInfo("POST", "api/partnums/gen", "Part number category not provided.");
            return new APIResponse<PartNumber>("POST", "Part number category not provided.");
         }
         var newPartNumber = await _partNumberService.GeneratePartNumberAsync(requestData);
         return newPartNumber is null
            ? throw new ModelNotFoundException("PartNumber", "Part number not created.")
            : new APIResponse<PartNumber>(newPartNumber, "POST");
      }
      catch (ModelNotFoundException e)
      {
         _logger.ApiLogError("POST", "api/partnums/gen", e.Message, e);
         return new APIResponse<PartNumber>("POST", e.Message);
      }
      catch (Exception e)
      {
         _logger.ApiLogError("POST", "api/partnums/gen", e.Message, e);
         return new APIResponse<PartNumber>("POST", e.Message);
      }
   }

   /// <summary>
   /// Update a part number in the database.
   /// <list type="table">
   ///   <item>
   ///      <term>PUT</term>
   ///      <description>api/partnums/{<paramref name="id"/>}</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="PartNumberRequestModel"/> <paramref name="updatedPartNumber"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="id"><see cref="ObjectId"/> of the part to update.</param>
   /// <param name="updatedPartNumber">Updated <see cref="PartNumber"/>.</param>
   [HttpPut("{id:length(24)}")]
   public async Task<APIResponse<bool>> Put(string id, [FromBody] PartNumber updatedPartNumber)
   {
      try
      {
         if (updatedPartNumber is null)
         {
            _logger.ApiLogInfo("PUT", "api/partnums/{id}", "Part Number is null.");
            return new APIResponse<bool>(false, "PUT", "Part Number is null.");
         }

         return new(await _partNumberService.UpdateDatabaseAsync(id, updatedPartNumber), "PUT");
      }
      catch (Exception e)
      {
         _logger.ApiLogError("PUT", "api/partnums/{id}", e.Message, e);
         return new APIResponse<bool>("PUT", e.Message);
      }
   }

   /// <summary>
   /// Delete a part from the database.
   /// <list type="table">
   ///   <item>
   ///      <term>DELETE</term>
   ///      <description>api/partnums/{<paramref name="id"/>}</description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="id"><see cref="ObjectId"/> of the <see cref="PartNumber"/> to delete.</param>
   [HttpDelete("{id:length(24)}")]
   public async Task<APIResponse<bool>> Delete(string id)
   {
      try
      {
         return new(await _partNumberService.DeleteFromDatabaseAsync(id), "DELETE");
      }
      catch (Exception e)
      {
         _logger.ApiLogError("DELETE", "api/partnums/{id}", e.Message, e);
         return new("DELETE", e.Message);
      }
   }
}
