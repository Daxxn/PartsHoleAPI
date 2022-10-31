using PartsHoleAPI.Models.Interfaces;

namespace PartsHoleAPI.Models
{
   public class DatabaseSettings : IDatabaseSettings
   {
      #region Local Props
      public string ConnectionString { get; set; }

      public string DatabaseName { get; set; }

      public string BinCollection { get; set; }
      public string DatasheetCollection { get; set; }
      public string InvoiceCollection { get; set; }
      public string PartCollection { get; set; }
      public string UserCollection { get; set; }
      #endregion

      #region Constructors
      public DatabaseSettings() { }
      #endregion

      #region Methods
      public string GetCollectionString<T>() where T : IModel
      {
         if (typeof(T) is IBinModel)
         {
            return BinCollection;
         }
         else if (typeof(T) is IInvoiceModel)
         {
            return InvoiceCollection;
         }
         else if (typeof(T) is IPartModel)
         {
            return PartCollection;
         }
         else if (typeof(T) is IUserModel)
         {
            return UserCollection;
         }
         return DatabaseName;
      }
      #endregion

      #region Full Props

      #endregion
   }
}
