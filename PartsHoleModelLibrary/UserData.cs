using PartsHoleLib.Interfaces;

namespace PartsHoleLib;

public class UserData : IUserData
{
   public List<IPartModel> Parts { get; set; } = null!;
   public List<IInvoiceModel> Invoices { get; set; } = null!;
   public List<IBinModel> Bins { get; set; } = null!;
}
