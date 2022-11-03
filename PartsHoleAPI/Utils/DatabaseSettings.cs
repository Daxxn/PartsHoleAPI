using PartsHoleLib;
using PartsHoleLib.Interfaces;

using PartsHoleModelLibrary;

namespace PartsHoleAPI.Utils
{
   public class DatabaseSettings : IDatabaseSettings
   {
      #region Local Props
      public string ConnectionString { get; set; } = null!;

      public string Name { get; set; } = null!;

      public string PartsCollection { get; set; } = null!;
      public string UsersCollection { get; set; } = null!;
      public string BinCollection { get; set; } = null!;
      public string InvoiceCollection { get; set; } = null!;
      #endregion

      #region Constructors
      #endregion

      #region Methods
      public string? GetCollection<T>() where T : IModel
      {
         if (typeof(T) == typeof(IBinModel))
         {
            return BinCollection;
         }
         else if (typeof(T) == typeof(IInvoiceModel))
         {
            return InvoiceCollection;
         }
         else if (typeof(T) == typeof(IPartModel))
         {
            return PartsCollection;
         }
         else if (typeof(T) == typeof(IUserModel))
         {
            return UsersCollection;
         }
         return null!;
      }
      #endregion

      #region Full Props

      #endregion
   }
}
