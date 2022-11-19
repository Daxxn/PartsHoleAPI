using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;

using PartsHoleAPI.DBServices;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

using PartsHoleRestLibrary.Responses;

namespace PartsHoleAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoicesController : ControllerBase
{
   #region Local Props
   private readonly ICollectionService<IInvoiceModel> _collection;
   private readonly ILogger<InvoicesController> _logger;
   #endregion

   #region Constructors
   public InvoicesController(ICollectionService<IInvoiceModel> invoiceCollection, ILogger<InvoicesController> logger)
   {
      _collection = invoiceCollection;
      _logger = logger;
   }
   #endregion

   #region API Methods
   /// <summary>
   /// Gets an <see cref="IInvoiceModel"/> based on the given <see cref="ObjectId"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>GET</term>
   ///      <description>api/invoices/{<paramref name="id"/>}</description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="id"><see cref="ObjectId"/> to search for.</param>
   /// <returns><see cref="IInvoiceModel"/> found. Null if unable.</returns>
   [HttpGet("{id:length(24)}")]
   public async Task<ActionResult<IPartModel?>> Get(string id)
   {
      if (string.IsNullOrEmpty(id))
         return BadRequest();
      return Ok(await _collection.GetFromDatabaseAsync(id));
   }

   /// <summary>
   /// Creates a new <see cref="IInvoiceModel"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/invoices</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="IInvoiceModel"/> <paramref name="newInvoice"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="newInvoice">New <see cref="IInvoiceModel"/> to create.</param>
   /// <returns><see langword="true"/> if successful, otherwise <see langword="false"/>.</returns>
   [HttpPost]
   public async Task<ActionResult<APIResponse<bool>>> Post([FromBody] InvoiceModel newInvoice)
   {
      try
      {
         if (newInvoice is null)
         {
            _logger.LogWarning("Bad Request : invoice is null.");
            return BadRequest(new APIResponse<bool>(false, "POST", "Invoice is null"));
         }
         return Ok(new APIResponse<bool>(await _collection.AddToDatabaseAsync(newInvoice), "POST"));
      }
      catch (Exception e)
      {
         _logger.LogError($"Unknown Error : {e.Message}", e);
         throw;
      }
   }

   /// <summary>
   /// Creates multiple <see cref="IInvoiceModel"/>s.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/invoices/many</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="List{T}"/> of <see cref="IInvoiceModel"/> <paramref name="newInvoices"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="newInvoices"><see cref="List{T}"/> of new <see cref="IInvoiceModel"/>s to create.</param>
   /// <returns><see cref="List{T}"/> of <see cref="bool"/>s where; <see langword="true"/> if successful, otherwise <see langword="false"/>.</returns>
   [HttpPost("many")]
   public async Task<ActionResult<APIResponse<IEnumerable<bool>?>>> PostMany([FromBody] InvoiceModel[] newInvoices)
   {
      if (newInvoices is null)
      {
         _logger.LogWarning("Invoice array is null.");
         return BadRequest(new APIResponse<IEnumerable<bool>?>("POST", "Invoice array is null."));
      }
      var response = await _collection.AddToDatabaseAsync(newInvoices);
      if (response is null)
      {
         _logger.LogWarning("Database did not return any data. Should be IEnumerable<bool>.");
         return Ok(new APIResponse<IEnumerable<bool>?>("POST", "Database did not respond."));
      }
      _logger.LogInformation("Database did not return any data. Should be IEnumerable<bool>.");
      return Ok(new APIResponse<IEnumerable<bool>?>(response, "POST"));
   }

   /// <summary>
   /// Updates an <see cref="IInvoiceModel"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>PUT</term>
   ///      <description>api/invoices/{<paramref name="id"/>}</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="IInvoiceModel"/> <paramref name="updatedInvoice"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="id"><see cref="ObjectId"/> of the <see cref="IInvoiceModel"/> to update.</param>
   /// <param name="updatedInvoice">Updated <see cref="IInvoiceModel"/> data.</param>
   /// <returns><see langword="true"/> if successful, otherwise <see langword="false"/></returns>
   [HttpPut("{id:length(24)}")]
   public async Task<ActionResult<APIResponse<bool>>> Put(string id, [FromBody] InvoiceModel updatedInvoice)
   {
      if (string.IsNullOrEmpty(id))
         return BadRequest(new APIResponse<bool>(false, "PUT", "ID not found"));
      if (id.Length != 24)
         return BadRequest(new APIResponse<bool>(false, "PUT", "ID not valid"));
      return Ok(new APIResponse<bool>(await _collection.UpdateDatabaseAsync(id, updatedInvoice), "PUT"));
   }

   /// <summary>
   /// Delete an <see cref="IInvoiceModel"/> from the database.
   /// <list type="table">
   ///   <item>
   ///      <term>DELETE</term>
   ///      <description>api/invoices/{<paramref name="id"/>}</description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="id">The <see cref="ObjectId"/> of the <see cref="IInvoiceModel"/> to delete.</param>
   /// <returns><see langword="true"/> if successful, otherwise <see langword="false"/></returns>
   [HttpDelete("{id:length(24)}")]
   public async Task<ActionResult<APIResponse<bool>>> Delete(string id)
   {
      if (string.IsNullOrEmpty(id))
         return BadRequest(new APIResponse<bool>(false, "DELETE", "ID not found"));
      if (id.Length != 24)
         return BadRequest(new APIResponse<bool>(false, "DELETE", "ID not valid"));
      return Ok(new APIResponse<bool>(await _collection.DeleteFromDatabaseAsync(id), "DELETE"));
   }

   // DELETE api/Invoices/many
   // Body : string[] ids
   /// <summary>
   /// Delete multiple <see cref="IInvoiceModel"/>s from the database.
   /// <list type="table">
   ///   <item>
   ///      <term>DELETE</term>
   ///      <description>api/invoices/many</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="List{T}"/> <see cref="ObjectId"/> <paramref name="ids"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="ids"><see cref="List{T}"/> of <see cref="ObjectId"/>s to delete.</param>
   /// <returns>Number (<see cref="int"/>) of <see cref="IInvoiceModel"/>s successfully deleted.</returns>
   [HttpDelete("many")]
   public async Task<ActionResult<APIResponse<int>>> DeleteMany(string[] ids)
   {
      if (ids is null)
         return BadRequest(new APIResponse<int>(0, "DELETE", "IDs not found"));
      if (ids.Length == 0)
         return BadRequest(new APIResponse<int>(0, "DELETE", "IDs empty"));
      return Ok(new APIResponse<int>(await _collection.DeleteFromDatabaseAsync(ids), "DELETE"));
   }
   #endregion
}
