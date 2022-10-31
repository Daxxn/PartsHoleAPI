using Microsoft.AspNetCore.Mvc.ApplicationModels;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using PartsHoleAPI.Models.Interfaces;

namespace PartsHoleAPI.Models
{
   public class PartModel : IPartModel
   {
      #region Local
      [BsonId]
      [BsonRepresentation(BsonType.ObjectId)]
      public string Id { get; set; }
      public string SupplierPartNumber { get; set; }
      public string PartNumber { get; set; }
      public string? Description { get; set; }
      public string? Reference { get; set; }
      public uint Quantity { get; set; }
      public uint AllocatedQty { get; set; }
      public uint Slippage { get; set; }
      public decimal UnitPrice { get; set; }
      public decimal ExtendedPrice { get; set; }
      public uint Backorder { get; set; }
      public IDatasheetModel Datasheet { get; set; }
      #endregion

      #region Constructors
      public PartModel() { }
      #endregion

      #region Methods
      #endregion

      #region Full Props

      #endregion
   }
}
