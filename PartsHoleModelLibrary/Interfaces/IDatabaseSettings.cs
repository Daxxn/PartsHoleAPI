namespace PartsHoleLib.Interfaces
{
   public interface IDatabaseSettings
   {
      string BinsCollection { get; set; }
      string ConnectionString { get; set; }
      string Name { get; set; }
      string InvoiceCollection { get; set; }
      string PartsCollection { get; set; }
      string UsersCollection { get; set; }
   }
}