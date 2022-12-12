using System.Text.Json.Serialization;

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
   public string SupplierPartNumber { get; set; } = null!;

   [CSVProperty("MANUFACTURER PART NUMBER")]
   [ExcelProperty("Mfr. #:")]
   public string PartNumber { get; set; } = null!;

   [CSVProperty("DESCRIPTION")]
   [ExcelProperty("Desc.:")]
   public string Description { get; set; } = null!;

   [CSVProperty("CUSTOMER REFERENCE")]
   [ExcelProperty("Customer #")]
   public string? Reference { get; set; }

   [CSVProperty("BACKORDER")]
   [ExcelIgnore]
   public int Backorder { get; set; }

   [CSVProperty("UNIT PRICE")]
   [ExcelProperty("Price (USD)")]
   public decimal UnitPrice { get; set; }

   public bool AddToInventory { get; set; }
   #endregion

   #region Constructors
   public InvoicePartModel() => Id = ObjectId.GenerateNewId().ToString();
   #endregion

   #region Methods
   public override string ToString() =>
      $"{(Id is null ? "" : "'ID'")} {PartNumber} {SupplierPartNumber} {Reference} {Quantity} {UnitPrice} {Description}";
   #endregion

   #region Other Props
   [ExcelIgnore]
   [JsonIgnore]
   public decimal ExtendedPrice => UnitPrice * Quantity;
   #endregion
}
