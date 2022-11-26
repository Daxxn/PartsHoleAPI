using System.Collections.Concurrent;

using CSVParserLibrary;

using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Driver;
using PartsHoleAPI.DBServices.Interfaces;
using PartsHoleAPI.Utils;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.DBServices;

public class InvoiceService : IInvoiceService
{
   #region Local Props
   public IMongoCollection<InvoiceModel> Collection { get; init; }
   private readonly IAbstractFactory<ICSVParser> _parserFactory;
   private readonly ICSVParserOptions _parserOptions;
   #endregion

   #region Constructors
   public InvoiceService(
      IOptions<DatabaseSettings> settings,
      IAbstractFactory<ICSVParser> parserFactory)
   {
      _parserFactory = parserFactory;
      _parserOptions = new CSVParserOptions()
      {
         IgnoreCase = true,
         IgnoreLineParseErrors = true,
      };
      var str = settings.Value.GetCollection<InvoiceModel>();
      var client = new MongoClient(settings.Value.ConnectionString);
      Collection = client.GetDatabase(settings.Value.DatabaseName).GetCollection<InvoiceModel>(str);
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
      if (int.TryParse(Path.GetFileNameWithoutExtension(file.FileName), out var id))
      {
         var parser = _parserFactory.Create();
         var result = await parser.ParseFileAsync<DigiKeyPartModel>(file.OpenReadStream(), _parserOptions);
         var newInvoice = new InvoiceModel()
         {
            Parts = result.Values.ToList(),
            OrderNumber = id,
         };
         var foundInvoices = (await Collection.FindAsync((inv) => inv.OrderNumber == id)).FirstOrDefault();
         if (foundInvoices != null)
         {
            if ((await Collection.ReplaceOneAsync((inv) => inv.OrderNumber == id, newInvoice)).IsAcknowledged)
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
      throw new Exception("File name must be the same as the sales order number from DigiKey.");
   }

   public async Task<IEnumerable<InvoiceModel>> ParseInvoiceFilesAsync(IEnumerable<IFormFile> files)
   {
      var bag = new ConcurrentBag<InvoiceModel>();
      var errors = new List<Exception>();
      await Parallel.ForEachAsync(files, async (file, token) =>
      {
         try
         {
            if (int.TryParse(Path.GetFileNameWithoutExtension(file.Name), out int orderNum))
            {
               var parser = _parserFactory.Create();
               var results = await parser.ParseFileAsync<DigiKeyPartModel>(file.OpenReadStream(), _parserOptions);
               var newInvoice = new InvoiceModel()
               {
                  OrderNumber = orderNum,
                  Parts = new(results.Values)
               };
               if (results.Errors != null)
               {
                  errors.AddRange(results.Errors);
               }
               bag.Add(newInvoice);
            }
         }
         catch (Exception e)
         {
            errors.Add(e);
         }
      });
      return errors.Count == 0 ? bag : throw new AggregateException(errors);
   }
   #endregion

   #region Full Props

   #endregion
}
