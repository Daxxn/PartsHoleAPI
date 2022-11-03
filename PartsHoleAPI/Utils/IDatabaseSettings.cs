namespace PartsHoleAPI.Utils
{
   public interface IDatabaseSettings
   {
      string ConnectionString { get; set; }
      string Name { get; set; }

      string UsersCollection { get; set; }
      string PartsCollection { get; set; }
      string InvoiceCollection { get; set; }
      string BinCollection { get; set; }
   }
}