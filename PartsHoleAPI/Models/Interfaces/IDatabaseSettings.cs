namespace PartsHoleAPI.Models.Interfaces
{
    public interface IDatabaseSettings
    {
        string BinCollection { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string DatasheetCollection { get; set; }
        string InvoiceCollection { get; set; }
        string PartCollection { get; set; }
        string UserCollection { get; set; }
    }
}