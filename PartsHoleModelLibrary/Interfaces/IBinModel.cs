using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace PartsHoleLib.Interfaces;

public interface IBinModel : IModel
{
   [BsonId]
   [BsonRepresentation(BsonType.ObjectId)]
   new string _id { get; set; }
   int Horizontal { get; set; }
   bool IsBook { get; set; }
   string Name { get; set; }
   int Vertical { get; set; }
}