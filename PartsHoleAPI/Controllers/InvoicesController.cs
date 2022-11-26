using System.Collections;

using CSVParserLibrary;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using MongoDB.Bson;
using PartsHoleAPI.DBServices.Interfaces;
using PartsHoleAPI.Utils;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

using PartsHoleRestLibrary.Responses;

namespace PartsHoleAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoicesController : ControllerBase
{
   #region Local Props
   private readonly IInvoiceService _invoiceService;
   private readonly ILogger<InvoicesController> _logger;
   #endregion

   #region Constructors
   public InvoicesController(
      IInvoiceService invoiceService,
      ILogger<InvoicesController> logger)
   {
      _invoiceService = invoiceService;
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
   public async Task<APIResponse<InvoiceModel>> Get(string id)
   {
      try
      {
         var foundInvoice = await _invoiceService.GetFromDatabaseAsync(id);
         if (foundInvoice != null)
         {
            _logger.ApiLogInfo("GET", "api/invoices", $"Invoice {id} found.");
            return new(foundInvoice, "GET");
         }
         _logger.ApiLogWarn("GET", "api/invoices", "Invoice not found.");
         return new("GET", "Invoice not found.");
      }
      catch (Exception e)
      {
         _logger.ApiLogError("GET", "api/invoices", "Internal Error", e);
         throw;
      }
   }

   /// <summary>
   /// Creates a new <see cref="InvoiceModel"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/invoices</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="InvoiceModel"/> <paramref name="newInvoice"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="newInvoice">New <see cref="InvoiceModel"/> to create.</param>
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
         return Ok(new APIResponse<bool>(await _invoiceService.AddToDatabaseAsync(newInvoice), "POST"));
      }
      catch (Exception e)
      {
         _logger.LogError($"Unknown Error : {e.Message}", e);
         throw;
      }
   }

   /// <summary>
   /// Creates multiple <see cref="InvoiceModel"/>s.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/invoices/many</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="List{T}"/> of <see cref="InvoiceModel"/> <paramref name="newInvoices"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="newInvoices"><see cref="List{T}"/> of new <see cref="InvoiceModel"/>s to create.</param>
   /// <returns><see cref="List{T}"/> of <see cref="bool"/>s where; <see langword="true"/> if successful, otherwise <see langword="false"/>.</returns>
   [HttpPost("many")]
   public async Task<ActionResult<APIResponse<IEnumerable<bool>?>>> PostMany([FromBody] InvoiceModel[] newInvoices)
   {
      if (newInvoices is null)
      {
         _logger.LogWarning("Invoice array is null.");
         return BadRequest(new APIResponse<IEnumerable<bool>?>("POST", "Invoice array is null."));
      }
      var response = await _invoiceService.AddToDatabaseAsync(newInvoices);
      if (response is null)
      {
         _logger.LogWarning("Database did not return any data. Should be IEnumerable<bool>.");
         return Ok(new APIResponse<IEnumerable<bool>?>("POST", "Database did not respond."));
      }
      _logger.LogInformation("Database did not return any data. Should be IEnumerable<bool>.");
      return Ok(new APIResponse<IEnumerable<bool>?>(response, "POST"));
   }

   /// <summary>
   /// Updates an <see cref="InvoiceModel"/>.
   /// <list type="table">
   ///   <item>
   ///      <term>PUT</term>
   ///      <description>api/invoices</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="InvoiceModel"/> <paramref name="updatedInvoice"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="updatedInvoice">Updated <see cref="InvoiceModel"/> data.</param>
   /// <returns><see langword="true"/> if successful, otherwise <see langword="false"/></returns>
   [HttpPut]
   public async Task<ActionResult<APIResponse<bool>>> Put([FromBody] InvoiceModel updatedInvoice)
   {
      if (updatedInvoice is null)
         return BadRequest(new APIResponse<bool>(false, "PUT", "Method body not found"));
      if (string.IsNullOrEmpty(updatedInvoice._id))
         return BadRequest(new APIResponse<bool>(false, "PUT", "ID not found"));
      return updatedInvoice._id.Length != 24
       ? BadRequest(new APIResponse<bool>(false, "PUT", "ID not valid"))
       : Ok(new APIResponse<bool>(await _invoiceService.UpdateDatabaseAsync(updatedInvoice._id, updatedInvoice), "PUT"));
   }

   /// <summary>
   /// Delete an <see cref="InvoiceModel"/> from the database.
   /// <list type="table">
   ///   <item>
   ///      <term>DELETE</term>
   ///      <description>api/invoices/{<paramref name="id"/>}</description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="id">The <see cref="ObjectId"/> of the <see cref="InvoiceModel"/> to delete.</param>
   /// <returns><see langword="true"/> if successful, otherwise <see langword="false"/></returns>
   [HttpDelete("{id:length(24)}")]
   public async Task<ActionResult<APIResponse<bool>>> Delete(string id)
   {
      if (string.IsNullOrEmpty(id))
         return BadRequest(new APIResponse<bool>(false, "DELETE", "ID not found"));
      return id.Length != 24
       ? BadRequest(new APIResponse<bool>(false, "DELETE", "ID not valid"))
       : Ok(new APIResponse<bool>(await _invoiceService.DeleteFromDatabaseAsync(id), "DELETE"));
   }

   // DELETE api/Invoices/many
   // Body : string[] ids
   /// <summary>
   /// Delete multiple <see cref="InvoiceModel"/>s from the database.
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
   /// <returns>Number (<see cref="int"/>) of <see cref="InvoiceModel"/>s successfully deleted.</returns>
   [HttpDelete("many")]
   public async Task<ActionResult<APIResponse<int>>> DeleteMany(string[] ids)
   {
      if (ids is null)
         return BadRequest(new APIResponse<int>(0, "DELETE", "IDs not found"));
      return ids.Length == 0
       ? BadRequest(new APIResponse<int>(0, "DELETE", "IDs empty"))
       : Ok(new APIResponse<int>(await _invoiceService.DeleteFromDatabaseAsync(ids), "DELETE"));
   }

   /// <summary>
   /// Parses a DigiKey invoice <see cref="IFormFile"/>, saves the created <see cref="InvoiceModel"/> to the database and sends the completed <see cref="IInvoiceModel"/> back.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/invoices/files/single</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="IFormFile"/> <paramref name="file"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="file">The DigiKey invoice file.</param>
   /// <returns>New <see cref="InvoiceModel"/>.</returns>
   [HttpPost("files/single")]
   public async Task<ActionResult<APIResponse<InvoiceModel?>>> PostParseFile(IFormFile file)
   {
      if (file == null)
         return BadRequest(new APIResponse<InvoiceModel?>(null, "POST", "Unable to map file to parameter."));
      var invoice = await _invoiceService.ParseInvoiceFileAsync(file);
      return invoice == null
       ? BadRequest(new APIResponse<InvoiceModel?>(null, "POST", "Returned invoice is null."))
       : Ok(new APIResponse<InvoiceModel?>(invoice, "POST", ""));
   }

   /// <summary>
   /// Parses an <see cref="Array"/> of DigiKey invoice <see cref="IFormFile"/>s, adds them to the database, and sends the completed <see cref="IInvoiceModel"/>s back.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/invoices/files/many/{}</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="Array"/> of <see cref="IFormFile"/> <paramref name="files"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="files"><see cref="Array"/> of DigiKey invoice files.</param>
   /// <returns><see cref="Array"/> of new <see cref="InvoiceModel"/>s.</returns>
   [HttpPost("files/many")]
   public async Task<ActionResult<APIResponse<IEnumerable<InvoiceModel>?>>> PostParseManyFiles(IEnumerable<IFormFile> files)
   {
      if (files == null)
         return BadRequest(new APIResponse<IEnumerable<InvoiceModel>?>(null, "POST", $"Unable to map files to {nameof(files)} parameter."));
      var invoices = await _invoiceService.ParseInvoiceFilesAsync(files);
      return invoices == null
       ? BadRequest(new APIResponse<IEnumerable<InvoiceModel>?>(null, "POST", $"Internal error. Probable file parse issue. Returned invoices are null."))
       : Ok(new APIResponse<IEnumerable<InvoiceModel>?>(invoices, "POST"));
   }

   /// <summary>
   /// !!Testing ONLY!!
   /// <para/>
   /// Runs the parser syncrnously for testing the functionality.
   /// <list type="table">
   ///   <item>
   ///      <term>POST</term>
   ///      <description>api/invoices/files/test</description>
   ///   </item>
   ///   <item>
   ///      <term>BODY</term>
   ///      <description><see cref="IFormFile"/> <paramref name="file"/></description>
   ///   </item>
   /// </list>
   /// </summary>
   /// <param name="file">File to parse.</param>
   /// <returns></returns>
   [HttpPost("files/test")]
   public async Task<ActionResult<InvoiceModel?>> PostParseFileTest(IFormFile file)
   {
      if (file == null)
         return BadRequest("File did not map properly.");
      if (file.ContentType != "text/csv")
         return BadRequest("Invalid file type. File nust be a CSV file.");
      if (int.TryParse(Path.GetFileNameWithoutExtension(file.FileName), out int orderNum))
      {
         var newInvoice = await _invoiceService.ParseInvoiceFileAsync(file);
         return Ok(newInvoice);
      }
      return BadRequest("File name is not valid. Name must be the DigiKey sales order number.");
   }
   #endregion
}
