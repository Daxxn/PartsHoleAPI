namespace PartsHoleAPI.Models.Interfaces
{
    public interface IPartModel : IModel
    {
        string PartNumber { get; set; }
        string SupplierPartNumber { get; set; }
        string? Reference { get; set; }
        uint Quantity { get; set; }
        uint AllocatedQty { get; set; }
        uint Backorder { get; set; }
        uint Slippage { get; set; }
        string? Description { get; set; }
        decimal ExtendedPrice { get; }
        decimal UnitPrice { get; set; }
        string? Datasheet { get; set; }
    }
}