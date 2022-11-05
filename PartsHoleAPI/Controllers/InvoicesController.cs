using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using PartsHoleAPI.DBServices;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

using PartsHoleRestLibrary.Responses;

namespace PartsHoleAPI.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class InvoicesController : ControllerBase
   {
      #region Local Props
      private readonly ICollectionService<IInvoiceModel> _collection;
      private readonly ILogger<InvoiceCollection> _logger;
      #endregion

      #region Constructors
      public InvoicesController(ICollectionService<IInvoiceModel> invoiceCollection, ILogger<InvoiceCollection> logger)
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
         _logger.Log(LogLevel.Debug, "Attempt to call generic GET method.");
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
      public async Task<ActionResult> Post([FromBody] InvoiceModel newInvoice)
      {
         if (newInvoice is null)
            return BadRequest(false);
         return Ok(await _collection.AddToDatabaseAsync(newInvoice));
      }

      // POST api/Invoices/many
      // Body : InvoiceModel[] newInvoices
      [HttpPost("many")]
      public async Task<ActionResult> PostMany([FromBody] InvoiceModel[] newInvoices)
      {
         if (newInvoices is null)
            return BadRequest();
         return Ok(await _collection.AddToDatabaseAsync(newInvoices));
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
}
