using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace PartsHoleLib.Interfaces
{
   public interface IModel
   {
      [BsonId]
      [BsonRepresentation(BsonType.ObjectId)]
      string? Id { get; set; }
   }
}
