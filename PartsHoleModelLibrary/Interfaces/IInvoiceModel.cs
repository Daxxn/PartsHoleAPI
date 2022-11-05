using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

using PartsHoleLib.Enums;

namespace PartsHoleLib.Interfaces
{
   public interface IInvoiceModel : IModel
   {
      int OrderNumber { get; set; }
      [BsonRepresentation(BsonType.ObjectId)]
      List<string> Parts { get; set; }
      string? Path { get; set; }
      decimal SubTotal { get; set; }
      int? SupplierType { get; set; }
      bool IsAddedToParts { get; set; }
   }
}