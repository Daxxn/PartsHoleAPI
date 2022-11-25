using Microsoft.AspNetCore.Mvc;
using PartsHoleAPI.DBServices.Interfaces;
using PartsHoleLib;
using PartsHoleLib.Interfaces;

using PartsHoleRestLibrary.Exceptions;
using PartsHoleRestLibrary.Requests;
using PartsHoleRestLibrary.Responses;

namespace PartsHoleAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PartNumController : ControllerBase
{
   #region Props
   private readonly IUserService _userService;
   private readonly ICollectionService<PartModel> _partsService;
   #endregion
   public PartNumController(
      IUserService userService,
      ICollectionService<PartModel> partsService
      )
   {
      _userService = userService;
      _partsService = partsService;
   }

   /// <summary>
   /// !!NOT FINISHED!!
   /// Generate New part number based on the provided.
   /// </summary>
   /// <param name="requestData">Type and SubType request data.</param>
   [HttpPost]
   public async Task<APIResponse<string>?> PostGeneratePartNumber([FromBody] PartNumberRequestModel requestData)
   {
      try
      {
         if (string.IsNullOrEmpty(requestData.UserId))
            return new APIResponse<string>("POST", "User ID not provided.");
         if (requestData.Type is null)
            return new APIResponse<string>("POST", "Part number type not provided.");
         if (requestData.SubType is null)
            return new APIResponse<string>("POST", "Part number sub-type not provided.");
         var foundUser = await _userService.GetFromDatabaseAsync(requestData.UserId);
         if (foundUser == null)
            throw new ModelNotFoundException("UserModel", "User not found.");
         var allParts = await _partsService.GetFromDatabaseAsync(foundUser.Parts.ToArray());
         if (allParts is null)
            throw new ModelNotFoundException("PartModel[]", "Part models not found.");
         var allPartNumbers = new PartNumberCollection(allParts.Select(x => PartNumber.Parse(x.PartNumber)));
         if (!allPartNumbers.Any())
            return new APIResponse<string>("POST", "No part numbers found.");

         allPartNumbers.ToList().Sort();
         throw new NotImplementedException();
      }
      catch (ModelNotFoundException e)
      {
         return new APIResponse<string>("POST", e.Message);
      }
      catch (Exception)
      {
         throw;
      }
   }

   // PUT api/<PartNumController>/5
   [HttpPut("{id}")]
   public void Put(int id, [FromBody] string value)
   {
   }

   // DELETE api/<PartNumController>/5
   [HttpDelete("{id}")]
   public void Delete(int id)
   {
   }
}
