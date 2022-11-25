using PartsHoleLib;

namespace PartsHoleAPI.DBServices.Interfaces;

public interface IInvoiceService : ICollectionService<InvoiceModel>
{
    Task<InvoiceModel> ParseInvoiceFileAsync(IFormFile file);
    Task<IEnumerable<InvoiceModel>> ParseInvoiceFilesAsync(IEnumerable<IFormFile> files);
}