using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

using PartsHoleLib.Enums;

namespace PartsHoleLib.Interfaces
{
   public interface IInvoiceModel
   {
      [BsonId]
      [BsonRepresentation(BsonType.ObjectId)]
      string? Id { get; set; }
      int OrderNumber { get; set; }
      List<IPartModel> Parts { get; set; }
      string? Path { get; set; }
      decimal SubTotal { get; set; }
      SupplierType? SupplierType { get; set; }
   }
}