using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PartsHoleAPI.Models.Interfaces;

namespace PartsHoleAPI.Models
{
   public class DatasheetModel : IDatasheetModel
   {
      [BsonId]
      [BsonRepresentation(BsonType.ObjectId)]
      public string Id { get; set; }
      public string? Path { get; set; }
   }
}
