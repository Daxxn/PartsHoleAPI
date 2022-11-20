using CSVParserLibrary.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PartsHoleLib;

public class DigiKeyPartModel
{
   #region Local Props
   [BsonId]
   [BsonRepresentation(BsonType.ObjectId)]
   public string Id { get; set; } = null!;

   [CSVProperty("QUANTITY")]
   public int Quantity { get; set; }

   [CSVProperty("PART NUMBER")]
   public string PartNumber { get; set; } = null!;

   [CSVProperty("MANUFACTURER PART NUMBER")]
   public string ManufacturerPartNumber { get; set; } = null!;

   [CSVProperty("DESCRIPTION")]
   public string Description { get; set; } = null!;

   [CSVProperty("CUSTOMER REFERENCE")]
   public string CustomerReference { get; set; } = null!;

   [CSVProperty("BACKORDER")]
   public int Backorder { get; set; }

   [CSVProperty("UNIT PRICE")]
   public decimal UnitPrice { get; set; }
   #endregion

   #region Constructors
   public DigiKeyPartModel() => Id = ObjectId.GenerateNewId().ToString();
   #endregion

   #region Methods
   public override string ToString() =>
      $"{(Id is null ? "" : "'ID'")} {ManufacturerPartNumber} {PartNumber} {CustomerReference} {Quantity} {UnitPrice} {Description}";
   #endregion

   #region Other Props
   public decimal ExtendedPrice => UnitPrice * Quantity;
   #endregion
}
