using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PartsHoleAPI.Models.Interfaces;

namespace PartsHoleAPI.Models
{
   public class BinModel : IBinModel
   {
      [BsonId]
      [BsonRepresentation(BsonType.ObjectId)]
      public string Id { get; set; }
      public string Name { get; set; } = "BIN";
      public int Horizontal { get; set; }
      public int Vertical { get; set; }
      public bool IsBook { get; set; }
   }
}
