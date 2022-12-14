using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using PartsHoleLib.Interfaces;

namespace PartsHoleLib;

public class PartModel : IModel
{
   #region Local
   [BsonId]
   [BsonRepresentation(BsonType.ObjectId)]
   public string _id { get; set; } = null!;
   public string SupplierPartNumber { get; set; } = null!;
   public string PartNumber { get; set; } = null!;
   public string? Description { get; set; }
   public string? Reference { get; set; }
   public uint Quantity { get; set; }
   public uint AllocatedQty { get; set; }
   public uint Slippage { get; set; }
   public decimal UnitPrice { get; set; }
   public uint Backorder { get; set; }
   public string? Datasheet { get; set; }

   [BsonRepresentation(BsonType.ObjectId)]
   public string? BinLocationId { get; set; }
   #endregion

   #region Constructors
   public PartModel() { }
   #endregion

   #region Methods
   public override string ToString() => $"{PartNumber} : {Reference} : {Quantity} : {Description:15}";
   #endregion

   #region Full Props
   public decimal ExtendedPrice => Quantity * UnitPrice;
   #endregion
}
