using System.Text.Json.Serialization;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using PartsHoleLib.Enums;
using PartsHoleLib.Interfaces;

namespace PartsHoleLib;

public class InvoiceModel : IModel
{
   [BsonId]
   [BsonRepresentation(BsonType.ObjectId)]
   public string Id { get; set; } = null!;
   public SupplierType SupplierType { get; set; }
   public List<InvoicePartModel> Parts { get; set; } = null!;
   public bool IsAddedToParts { get; set; }
   public uint OrderNumber { get; set; }

   public InvoiceModel() => Id = ObjectId.GenerateNewId().ToString();

   [BsonIgnore]
   [JsonIgnore]
   public decimal SubTotal
   {
      get
      {
         if (Parts is null)
            return 0;
         if (!Parts.Any())
            return 0;
         return Parts.Sum(p => p.ExtendedPrice);
      }
   }
}
