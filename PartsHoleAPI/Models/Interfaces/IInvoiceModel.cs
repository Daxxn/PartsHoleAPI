using PartsInventory.Models.Enums;

namespace PartsHoleAPI.Models.Interfaces
{
    public interface IInvoiceModel : IModel
    {
        int OrderNumber { get; set; }
        List<PartModel> Parts { get; set; }
        string? Path { get; set; }
        decimal SubTotal { get; set; }
        SupplierType? SupplierType { get; set; }
    }
}