using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PartsHoleLib.Interfaces;
using PartsHoleLib.Enums;

namespace PartsHoleLib
{
   public class InvoiceModel : IInvoiceModel
   {
      [BsonId]
      [BsonRepresentation(BsonType.ObjectId)]
      public string Id { get; set; } = null!;
      public List<IPartModel> Parts { get; set; } = null!;
      public SupplierType? SupplierType { get; set; }
      public string? Path { get; set; }
      public int OrderNumber { get; set; }
      public decimal SubTotal { get; set; }
   }
}
