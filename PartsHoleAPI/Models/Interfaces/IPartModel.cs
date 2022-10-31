namespace PartsHoleAPI.Models.Interfaces
{
    public interface IPartModel : IModel
    {
        uint AllocatedQty { get; set; }
        uint Backorder { get; set; }
        IDatasheetModel Datasheet { get; set; }
        string? Description { get; set; }
        decimal ExtendedPrice { get; set; }
        string PartNumber { get; set; }
        uint Quantity { get; set; }
        string? Reference { get; set; }
        uint Slippage { get; set; }
        string SupplierPartNumber { get; set; }
        decimal UnitPrice { get; set; }
    }
}