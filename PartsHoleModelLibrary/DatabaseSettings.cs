using Microsoft.Extensions.Options;

using PartsHoleAPI.Models.Interfaces;

namespace PartsHoleAPI.Models
{
   public class DatabaseSettings : IDatabaseSettings
   {
      #region Local Props
      public string ConnectionString { get; set; } = null!;

      public string Name { get; set; } = null!;

      public string PartsCollection { get; set; } = null!;
      public string UsersCollection { get; set; } = null!;
      public string BinsCollection { get; set; } = null!;
      public string InvoiceCollection { get; set; } = null!;
      #endregion

      #region Constructors
      #endregion

      #region Methods
      #endregion

      #region Full Props

      #endregion
   }
}
