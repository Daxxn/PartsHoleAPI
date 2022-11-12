using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
   // GET: api/Invoices
   [HttpGet]
   public ActionResult<string> Get()
   {
      _logger.LogInformation("Attempt to call generic GET method.");
      return Ok("Not allowed. A part ID is required.");
   }

   // GET api/Invoices/{id}
   [HttpGet("{id:length(24)}")]
   public async Task<ActionResult<IPartModel?>> Get(string id)
   {
      if (string.IsNullOrEmpty(id))
         return BadRequest();
      return Ok(await _collection.GetFromDatabaseAsync(id));
   }

   // POST api/Invoices
   // Body : InvoiceModel newInvoice
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

   // POST api/Invoices/many
   // Body : InvoiceModel[] newInvoices
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

   // PUT api/Invoices/{id}
   // Body : InvoiceModel updatedInvoice
   [HttpPut("{id:length(24)}")]
   public async Task<ActionResult<APIResponse<bool>>> Put(string id, [FromBody] InvoiceModel updatedInvoice)
   {
      if (string.IsNullOrEmpty(id))
         return BadRequest(new APIResponse<bool>(false, "PUT", "ID not found"));
      if (id.Length != 24)
         return BadRequest(new APIResponse<bool>(false, "PUT", "ID not valid"));
      return Ok(new APIResponse<bool>(await _collection.UpdateDatabaseAsync(id, updatedInvoice), "PUT"));
   }

   // DELETE api/Invoices/{id}
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
