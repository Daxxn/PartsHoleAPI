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
      public string _id { get; set; } = null!;
      public List<DigiKeyPartModel> Parts { get; set; } = null!;
      public int OrderNumber { get; set; }

      public InvoiceModel()
      {
         _id = ObjectId.GenerateNewId().ToString();
      }
      public decimal SubTotal
      {
         get
         {
            if (Parts is null) return 0;
            if (!Parts.Any()) return 0;
            return Parts.Sum(p => p.ExtendedPrice);
         }
      }
   }
}
