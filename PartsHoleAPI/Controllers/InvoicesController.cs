using System.Collections;

using CSVParserLibrary;

using Microsoft.AspNetCore.Authorization;
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
   public async Task<APIResponse<bool>> Post([FromBody] InvoiceModel newInvoice)
   {
      try
      {
         if (newInvoice is null)
         {
            _logger.ApiLogWarn("POST", "api/invoices", "Invoice is null.");
            return new(false, "POST", "Invoice is null");
         }
         _logger.ApiLogInfo("POST", "api/invoices", "New invoice created.");
         return new(await _invoiceService.AddToDatabaseAsync(newInvoice), "POST");
      }
      catch (Exception e)
      {
         _logger.ApiLogError("POST", "api/invoices", "Unknown Error", e);
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
   public async Task<APIResponse<IEnumerable<bool>?>> PostMany([FromBody] InvoiceModel[] newInvoices)
   {
      try
      {
         if (newInvoices is null)
         {
            _logger.ApiLogWarn("POST", "api/invoices/many", "Invoice array is null.");
            return new("POST", "Invoices are null.");
         }
         var response = await _invoiceService.AddToDatabaseAsync(newInvoices);
         if (response is null)
         {
            _logger.ApiLogWarn("POST", "api/invoices/many", "Database did not return any data.");
            return new("POST", "Database did not respond.");
         }
         _logger.ApiLogInfo("POST", "api/invoices/many", $"{response.Count()} invoices created.");
         return new(response, "POST");
      }
      catch (Exception e)
      {
         _logger.ApiLogError("POST", "api/invoices/many", "Unknown Error", e);
         throw;
      }
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
   public async Task<APIResponse<bool>> Put([FromBody] InvoiceModel updatedInvoice)
   {
      try
      {
         if (updatedInvoice is null)
         {
            _logger.ApiLogWarn("PUT", "api/invoices", "Body is null.");
            return new(false, "PUT", "Method body not found");
         }
         if (string.IsNullOrEmpty(updatedInvoice._id))
         {
            _logger.ApiLogWarn("PUT", "api/invoices", "Invoice ID is null.");
            return new(false, "PUT", "ID not found");
         }
         if (updatedInvoice._id.Length != 24)
         {
            _logger.ApiLogWarn("PUT", "api/invoices", "Invoice ID is not valid.");
            return new(false, "PUT", "Invoice ID is not valid.");
         }
         if (await _invoiceService.UpdateDatabaseAsync(updatedInvoice._id, updatedInvoice))
         {
            _logger.ApiLogInfo("PUT", "api/invoices", $"Invoice {updatedInvoice._id} updated.");
            return new(true, "PUT");
         }
         _logger.ApiLogWarn("PUT", "api/invoices", $"Failed to update invoice {updatedInvoice._id}.");
         return new(false, "PUT", "Unable to update invoice.");
      }
      catch (Exception e)
      {
         _logger.ApiLogError("PUT", "api/invoices", "Unknown Error", e);
         throw;
      }
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
   public async Task<APIResponse<bool>> Delete(string id)
   {
      try
      {
         if (await _invoiceService.DeleteFromDatabaseAsync(id))
         {
            _logger.ApiLogInfo("DELETE", "api/invoices/{id}", $"Sucessfully deleted {id}.");
            return new(true, "DELETE");
         }
         _logger.ApiLogWarn("DELETE", "api/invoices/{id}", $"Unable to delete invoice {id}.");
         return new(false, "DELETE", "Unable to delete invoice.");
      }
      catch (Exception e)
      {
         _logger.ApiLogError("DELETE", "api/invoices/{id}", "Unknown Error", e);
         throw;
      }
   }

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
   public async Task<APIResponse<int>> DeleteMany(string[] ids)
   {
      try
      {
         if (ids is null)
         {
            _logger.ApiLogWarn("DELETE", "api/invoices/many", "Body is null.");
            return new(0, "DELETE", "Body was null.");
         }
         var result = await _invoiceService.DeleteFromDatabaseAsync(ids);
         if (result == 0)
         {
            _logger.ApiLogWarn("DELETE", "api/invoices/many", "Failed to delete any invoices.");
            return new(result, "DELETE", "Unable to delet any invoices.");
         }
         if (result != ids.Length)
         {
            _logger.ApiLogWarn("DELETE", "api/invoices/many", $"Failed to delete some invoices. Deleted {result}");
            return new(result, "DELETE", "Unable to delete some invoices.");
         }
         _logger.ApiLogInfo("DELETE", "api/invoices/many", $"Successfully deleted {result} invoices.");
         return new(result, "DELETE");
      }
      catch (Exception e)
      {
         _logger.ApiLogError("DELETE", "api/invoices/many", "Unknown Error", e);
         throw;
      }
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
   public async Task<APIResponse<InvoiceModel>> PostParseFile(IFormFile file)
   {
      try
      {
         if (file is null)
         {
            _logger.ApiLogWarn("POST", "api/invoices/files/single", "File is null.");
            return new("POST", "File is null.");
         }
         var parsedFile = await _invoiceService.ParseInvoiceFileAsync(file);
         if (parsedFile is null)
         {
            _logger.ApiLogWarn("POST", "api/invoices/files/single", $"Parsed invoice {file.FileName} is null.");
            return new("POST", "Parsed invoice is null.");
         }
         _logger.ApiLogInfo("POST", "api/invoices/files/single", $"Parsed invoice {file.FileName}.");
         return new(parsedFile, "POST");
      }
      catch (Exception e)
      {
         _logger.ApiLogError("POST", "api/invoices/files/single", "Unknown Error", e);
         throw;
      }
   }

   /// <summary>
   /// BROKEN - Probably something with <c>RestSharp</c>. Either way, I cant get this working. It gets to the method but the list is empty.
   /// <para/>
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
   public async Task<APIResponse<IEnumerable<InvoiceModel>>> PostParseManyFiles(IEnumerable<IFormFile> files)
   {
      try
      {
         if (files is null)
         {
            _logger.ApiLogWarn("POST", "api/invoices/files/many", "Files are null.");
            return new("POST", "Files were null.");
         }
         var parsedInvoices = await _invoiceService.ParseInvoiceFilesAsync(files);
         if (parsedInvoices is null)
         {
            _logger.ApiLogWarn("POST", "api/invoices/files/many", "Parsed invoices are null.");
            return new("POST", "Parsed invoices were null.");
         }
         _logger.ApiLogInfo("POST", "api/invoices/files/many", $"Parsed {files.Count()} invoices.");
         return new(parsedInvoices, "POST");
      }
      catch (Exception e)
      {
         _logger.ApiLogError("POST", "api/invoices/files/many", "Unknown Error", e);
         throw;
      }
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
   public async Task<APIResponse<InvoiceModel>> PostParseFileTest(IFormFile file)
   {
      if (file == null)
         return new("POST", "File didnt map properly.");
      if (file.ContentType != "text/csv")
         return new("POST", "Invalid file type. File nust be a CSV file.");
      if (int.TryParse(Path.GetFileNameWithoutExtension(file.FileName), out int orderNum))
      {
         var newInvoice = await _invoiceService.ParseInvoiceFileAsync(file);
         return new(newInvoice, "POST");
      }
      return new("POST", "File name is not valid. Name must be the DigiKey sales order number.");
   }
   #endregion
}
