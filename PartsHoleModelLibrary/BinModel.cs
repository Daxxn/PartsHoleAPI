using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using PartsHoleLib.Interfaces;

namespace PartsHoleLib;

public class BinModel : IBinModel
{
   [BsonId]
   [BsonRepresentation(BsonType.ObjectId)]
   public string _id { get; set; } = null!;
   public string Name { get; set; } = "BIN";
   public int Horizontal { get; set; }
   public int Vertical { get; set; }
   public bool IsBook { get; set; }
}
