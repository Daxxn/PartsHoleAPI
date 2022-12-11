using System.Collections.Concurrent;

using CSVParserLibrary;

using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Driver;

using PartsHoleAPI.DBServices.Interfaces;
using PartsHoleAPI.Utils;

using PartsHoleLib;
using PartsHoleLib.Enums;
using PartsHoleLib.Interfaces;

using SixLabors.Fonts.Tables.AdvancedTypographic;

namespace PartsHoleAPI.DBServices;

public class InvoiceService : IInvoiceService
{
   #region Local Props
   public IMongoCollection<InvoiceModel> Collection { get; init; }
   private readonly IAbstractFactory<ICSVParser> _parserFactory;
   private readonly ICSVParserOptions _parserOptions;
   private readonly IMouserParseService _mouserParseService;
   private readonly ILogger<InvoiceService> _logger;
   #endregion

   #region Constructors
   public InvoiceService(
      IOptions<DatabaseSettings> settings,
      IAbstractFactory<ICSVParser> parserFactory,
      IMouserParseService mouserParseService,
      ILogger<InvoiceService> logger)
   {
      _parserFactory = parserFactory;
      _mouserParseService = mouserParseService;
      _logger = logger;
      _parserOptions = new CSVParserOptions()
      {
         IgnoreCase = true,
         IgnoreLineParseErrors = true,
         ExclusionFunctions = new()
         {
            { "Totals-Exclusion", (props) => props.Length == 9 && props[7].ToLower() == "subtotal" }
         }
      };
      var str = settings.Value.GetCollection<InvoiceModel>();
      var client = new MongoClient(settings.Value.ConnectionString);
      Collection = client.GetDatabase(settings.Value.DatabaseName).GetCollection<InvoiceModel>(str);
      _logger = logger;
   }
   #endregion

   #region Methods
   public async Task<InvoiceModel?> GetFromDatabaseAsync(string id)
   {
      var result = await Collection.FindAsync(invoice => invoice._id == id);
      if (result is null)
         return null;
      var invoices = await result.ToListAsync();
      if (invoices is null)
         return null;
      if (invoices.Count == 0)
         return null;
      return invoices.Count > 1
       ? throw new Exception("Multiple invoices found with same ID. Something is horribly wrong!!")
       : invoices[0];
   }

   public async Task<IEnumerable<InvoiceModel>?> GetFromDatabaseAsync(string[] ids)
   {
      var result = await Collection.FindAsync(part => ids.Contains(part._id));
      return result?.ToEnumerable();
   }

   public async Task<bool> AddToDatabaseAsync(InvoiceModel data)
   {
      var filter = Builders<InvoiceModel>.Filter.Where(x => data._id == x._id);
      var result = await Collection.FindAsync(filter);
      if (result is null)
         throw new Exception("Find invoice failed.");
      if (result.FirstOrDefault() is null)
         return await AddToDatabaseAsync(data);
      var replaceResult = await Collection.ReplaceOneAsync(filter, data);
      return replaceResult is not null && replaceResult.ModifiedCount > 0;
   }

   public async Task<IEnumerable<bool>?> AddToDatabaseAsync(IEnumerable<InvoiceModel> data)
   {
      var invoiceData = data.ToList();
      var status = new List<bool>(invoiceData.Count);
      var ids = data.Select(x => x._id).ToList();
      var result = await Collection.FindAsync(part => ids.Contains(part._id));
      if (result is null)
         return null;
      if (result.ToList().Count > 0)
         return null;
      await Parallel.ForEachAsync(data, async (part, token) =>
      {
         if (token.IsCancellationRequested)
            return;
         var success = await AddToDatabaseAsync(part);
         var index = invoiceData.IndexOf(part);
         status.Insert(index, success);
      });
      return status;
   }

   public async Task<bool> UpdateDatabaseAsync(string id, InvoiceModel data)
   {
      var filter = Builders<InvoiceModel>.Filter.Where(x => id == x._id);
      var result = await Collection.FindAsync(filter);
      if (result is null)
         throw new Exception("Find failed.");
      if (result.FirstOrDefault() is null)
         return await AddToDatabaseAsync(data);
      var replaceResult = await Collection.ReplaceOneAsync(filter, data);
      return replaceResult is null ? false : replaceResult.ModifiedCount > 0;

   }

   public async Task<IEnumerable<bool>?> UpdateDatabaseAsync(IEnumerable<InvoiceModel> data)
   {
      var partData = data.ToList();
      var results = new List<bool>(partData.Count);
      await Parallel.ForEachAsync(data, async (d, token) =>
      {
         if (token.IsCancellationRequested)
            return;
         var success = false;
         if (!string.IsNullOrEmpty(d._id))
         {
            success = await UpdateDatabaseAsync(d._id, d);
         }
         var index = partData.IndexOf(d);
         results.Insert(index, success);
      });
      return results;
   }

   public async Task<bool> DeleteFromDatabaseAsync(string id)
   {
      var result = await Collection.DeleteOneAsync((p) => p._id == id);
      return result is not null && result.DeletedCount > 0;
   }

   public async Task<int> DeleteFromDatabaseAsync(string[] ids)
   {
      var result = await Collection.DeleteManyAsync((p) => ids.Contains(p._id));
      return result is null ? 0 : (int)result.DeletedCount;
   }

   public async Task<InvoiceModel> ParseInvoiceFileAsync(IFormFile file)
   {
      var fileExt = Path.GetExtension(file.FileName);
      InvoiceModel? newInvoice = null;
      if (fileExt == ".csv")
      {
         var result = await ParseDigikeyInvoiceAsync(file);
         if (result != null)
         {
            newInvoice = result.Value.model;
            if (result.Value.errors != null && result.Value.errors.Any())
            {
               _logger.LogDebug("{count} Errors during parsing.", result.Value.errors.Count());
            }
         }
      }
      else if (fileExt == ".xls" || fileExt == ".xlsx")
      {
         var result = await ParseMouserInvoiceAsync(file);
         if (result != null)
         {
            newInvoice = result.Value.model;
            if (result.Value.errors != null && result.Value.errors.Any())
            {
               _logger.LogDebug("{count} Errors during parsing.", result.Value.errors.Count());
            }
         }
      }
      if (newInvoice is null)
      {
         throw new Exception("Failed to parse invoice.");
      }
      var foundInvoices = (await Collection.FindAsync((inv) => inv.OrderNumber == newInvoice.OrderNumber)).FirstOrDefault();
      if (foundInvoices != null)
      {
         if ((await Collection.ReplaceOneAsync((inv) => inv.OrderNumber == newInvoice.OrderNumber, newInvoice)).IsAcknowledged)
         {
            newInvoice._id = foundInvoices._id;
            return newInvoice;
         }
         throw new Exception("Invoice replacement failed. Replace failed to acknowledge.");
      }
      else
      {
         newInvoice._id = ObjectId.GenerateNewId().ToString();
         await Collection.InsertOneAsync(newInvoice);
         return newInvoice;
      }
   }

   /// <summary>
   /// Need to rethink. It doesnt add to the database.
   /// <para/>
   /// Its probably unecessary anyway. I cant get the client to send multiple files.
   /// </summary>
   /// <param name="files"></param>
   /// <returns></returns>
   /// <exception cref="AggregateException"></exception>
   public async Task<IEnumerable<InvoiceModel>> ParseInvoiceFilesAsync(IEnumerable<IFormFile> files)
   {
      var bag = new ConcurrentBag<InvoiceModel>();
      var errors = new List<Exception>();
      await Parallel.ForEachAsync(files, async (file, token) =>
      {
         try
         {
            var fileExt = Path.GetExtension(file.FileName);
            if (fileExt == ".csv")
            {
               var result = await ParseDigikeyInvoiceAsync(file);
               if (result != null)
               {
                  result.Value.model.SupplierType = SupplierType.DigiKey;
                  bag.Add(result.Value.model);
                  if (result.Value.errors != null)
                  {
                     errors.AddRange(result.Value.errors);
                  }
               }
            }
            else if (fileExt == ".xls" || fileExt == ".xlsx")
            {
               var result = await ParseMouserInvoiceAsync(file);
               if (result != null)
               {
                  result.Value.model.SupplierType = SupplierType.Mouser;
                  bag.Add(result.Value.model);
                  if (result.Value.errors != null)
                  {
                     errors.AddRange(result.Value.errors);
                  }
               }
            }
         }
         catch (Exception e)
         {
            errors.Add(e);
         }
      });
      return errors.Count == 0 ? bag : throw new AggregateException(errors);
   }

   private async Task<(InvoiceModel model, IEnumerable<Exception>? errors)?> ParseDigikeyInvoiceAsync(IFormFile file)
   {
      if (int.TryParse(Path.GetFileNameWithoutExtension(file.FileName), out int orderNum))
      {
         var parser = _parserFactory.Create();
         var results = await parser.ParseFileAsync<InvoicePartModel>(file.OpenReadStream(), _parserOptions);
         if (results is null)
            return null;
         var newInvoice = new InvoiceModel
         {
            OrderNumber = orderNum,
            SupplierType = SupplierType.DigiKey,
            Parts = new(results.Values)
         };
         return (newInvoice, results.Errors);
      }
      return null;
   }

   private async Task<(InvoiceModel model, IEnumerable<Exception>? errors)?> ParseMouserInvoiceAsync(IFormFile file)
   {
      if (int.TryParse(Path.GetFileNameWithoutExtension(file.FileName), out int orderNum))
      {
         var results = await _mouserParseService.ParseFileAsync(file);
         if (results is null)
            return null;
         var newInvoice = new InvoiceModel
         {
            OrderNumber = orderNum,
            SupplierType= SupplierType.Mouser,
            Parts = new(results.Data)
         };
         return (newInvoice, results.Errors);
      }
      return null;
   }
   #endregion

   #region Full Props

   #endregion
}
