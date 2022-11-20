using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PartsHoleLib.Interfaces
{
   public interface IInvoiceModel : IModel
   {
      [BsonId]
      [BsonRepresentation(BsonType.ObjectId)]
      new string _id { get; set; }
      int OrderNumber { get; set; }
      [BsonRepresentation(BsonType.ObjectId)]
      List<DigiKeyPartModel> Parts { get; set; }
      string? Path { get; set; }
      decimal SubTotal { get; set; }
      int? SupplierType { get; set; }
      bool IsAddedToParts { get; set; }
   }
}