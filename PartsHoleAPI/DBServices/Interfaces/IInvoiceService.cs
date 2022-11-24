using CSVParserLibrary;

using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.DBServices.Interfaces;

public interface IInvoiceService : ICollectionService<IInvoiceModel>
{
    Task<IInvoiceModel> ParseInvoiceFileAsync(IFormFile file);
    Task<IEnumerable<IInvoiceModel>> ParseInvoiceFilesAsync(IEnumerable<IFormFile> files);
}