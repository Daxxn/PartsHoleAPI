using ExcelParserLibrary;
using ExcelParserLibrary.Models;

using OfficeOpenXml;
using PartsHoleAPI.DBServices.Interfaces;
using PartsHoleAPI.Utils;

using PartsHoleLib;

namespace PartsHoleAPI.DBServices;

public class MouserParseService : IMouserParseService
{
   #region Local Props
   private readonly ILogger<MouserParseService> _logger;
   private readonly IAbstractFactory<IExcelParser> _excelParserFactory;
   #endregion

   #region Constructors
   public MouserParseService(ILogger<MouserParseService> logger, IAbstractFactory<IExcelParser> excelParserFactory)
   {
      _logger = logger;
      _excelParserFactory = excelParserFactory;
   }
   #endregion

   #region Methods
   public ExcelResult<InvoicePartModel> ParseFile(IFormFile file)
   {
      var fe = Path.GetExtension(file.FileName);
      if (fe != ".xls" && fe != ".xlsx")
         throw new ArgumentException("File is not a valid Excel file.");
      _logger.LogInformation("Starting File Parse: {file}", file.FileName);
      var parser = _excelParserFactory.Create();
      return parser.ParseFile<InvoicePartModel>(file.OpenReadStream());
   }

   public async Task<ExcelResult<InvoicePartModel>> ParseFileAsync(IFormFile file)
   {
      var fe = Path.GetExtension(file.FileName);
      if (fe != ".xls" && fe != ".xlsx")
         throw new ArgumentException("File is not a valid Excel file.");
      _logger.LogInformation("Starting File Parse: {file}", file.FileName);
      var parser = _excelParserFactory.Create();
      return await parser.ParseFileAsync<InvoicePartModel>(file.OpenReadStream());
   }
   #endregion

   #region Full Props

   #endregion
}
