using PartsHoleLib.Interfaces;

namespace PartsHoleLib;

public class UserData
{
   public List<PartModel> Parts { get; set; } = null!;
   public List<InvoiceModel> Invoices { get; set; } = null!;
   public List<BinModel> Bins { get; set; } = null!;
   public List<PartNumber> PartNumbers { get; set; } = null!;
}
