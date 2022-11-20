using PartsHoleLib.Interfaces;

namespace PartsHoleLib.Interfaces;

public interface IUserData
{
   List<IInvoiceModel> Invoices { get; set; }
   List<IPartModel> Parts { get; set; }
   List<IBinModel> Bins { get; set; }
}