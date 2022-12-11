using CSVParserLibrary.Models;

using ExcelParserLibrary.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PartsHoleLib;

public class InvoicePartModel
{
   #region Local Props
   [BsonId]
   [BsonRepresentation(BsonType.ObjectId)]
   [ExcelIgnore]
   public string Id { get; set; } = null!;

   [CSVProperty("QUANTITY")]
   [ExcelProperty("Order Qty.")]
   public int Quantity { get; set; }

   [CSVProperty("PART NUMBER")]
   [ExcelProperty("Mouser #:")]
   public string PartNumber { get; set; } = null!;

   [CSVProperty("MANUFACTURER PART NUMBER")]
   [ExcelProperty("Mfr. #:")]
   public string ManufacturerPartNumber { get; set; } = null!;

   [CSVProperty("DESCRIPTION")]
   [ExcelProperty("Desc.:")]
   public string Description { get; set; } = null!;

   [CSVProperty("CUSTOMER REFERENCE")]
   [ExcelProperty("Customer #")]
   public string CustomerReference { get; set; } = null!;

   [CSVProperty("BACKORDER")]
   [ExcelIgnore]
   public int Backorder { get; set; }

   [CSVProperty("UNIT PRICE")]
   [ExcelProperty("Price (USD)")]
   public decimal UnitPrice { get; set; }
   #endregion

   #region Constructors
   public InvoicePartModel() => Id = ObjectId.GenerateNewId().ToString();
   #endregion

   #region Methods
   public override string ToString() =>
      $"{(Id is null ? "" : "'ID'")} {ManufacturerPartNumber} {PartNumber} {CustomerReference} {Quantity} {UnitPrice} {Description}";
   #endregion

   #region Other Props
   [ExcelIgnore]
   public decimal ExtendedPrice => UnitPrice * Quantity;
   #endregion
}
