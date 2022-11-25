using PartsHoleLib;
using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.Utils
{
   public class DatabaseSettings
   {
      #region Local Props
      private string _connectionString = null!;
      public string ConnectionStringTemplate { get; set; } = null!;

      public string DatabaseName { get; set; } = null!;

      public string PartsCollection { get; set; } = null!;
      public string UsersCollection { get; set; } = null!;
      public string BinsCollection { get; set; } = null!;
      public string InvoicesCollection { get; set; } = null!;
      public string PartNumberCollection { get; set; } = null!;

      public string DevelopmentUserName { get; set; } = null!;
      public string DevelopmentPassword { get; set; } = null!;
      public string ProductionUserName { get; set; } = null!;
      public string ProductionPassword { get; set; } = null!;

      public DatabaseSettings() { }
      #endregion

      #region Methods
      public string? GetCollection<T>() where T : IModel
      {
         if (typeof(T) == typeof(BinModel))
         {
            return BinsCollection;
         }
         else if (typeof(T) == typeof(InvoiceModel))
         {
            return InvoicesCollection;
         }
         else if (typeof(T) == typeof(PartModel))
         {
            return PartsCollection;
         }
         else if (typeof(T) == typeof(UserModel))
         {
            return UsersCollection;
         }
         return null!;
      }
      #endregion

      #region Full Props
      public string ConnectionString
      {
         get
         {
            if (_connectionString is null)
            {
#if DEBUG
               _connectionString = ConnectionStringTemplate.Replace("<username>", DevelopmentUserName).Replace("<password>", DevelopmentPassword);
#else
               _connectionString = ConnectionStringTemplate.Replace("<username>", ProductionUserName).Replace("<password>", ProductionPassword);
#endif
            }
            return _connectionString;
         }
         set
         {
            _connectionString = value;
         }
      }
      #endregion
   }
}
