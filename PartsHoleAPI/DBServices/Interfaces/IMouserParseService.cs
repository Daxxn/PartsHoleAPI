using ExcelParserLibrary.Models;

using PartsHoleLib;

namespace PartsHoleAPI.DBServices.Interfaces
{
    public interface IMouserParseService
    {
        ExcelResult<InvoicePartModel> ParseFile(IFormFile file);
        Task<ExcelResult<InvoicePartModel>> ParseFileAsync(IFormFile file);
    }
}