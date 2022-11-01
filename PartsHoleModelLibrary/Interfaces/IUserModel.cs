namespace PartsHoleAPI.Models.Interfaces
{
   public interface IUserModel : IModel
   {
      List<IInvoiceModel> Invoices { get; set; }
      List<IPartModel> Parts { get; set; }
      string Username { get; set; }
      string AuthID { get; set; }
   }
}