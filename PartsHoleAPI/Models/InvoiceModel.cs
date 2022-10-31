using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PartsHoleAPI.Models.Interfaces;
using PartsInventory.Models.Enums;

namespace PartsHoleAPI.Models
{
   public class InvoiceModel : IInvoiceModel
   {
      [BsonId]
      [BsonRepresentation(BsonType.ObjectId)]
      public string Id { get; set; }
      public List<PartModel> Parts { get; set; }
      public SupplierType? SupplierType { get; set; }
      public string? Path { get; set; }
      public int OrderNumber { get; set; }
      public decimal SubTotal { get; set; }
   }
}
