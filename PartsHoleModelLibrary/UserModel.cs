using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using PartsHoleLib.Enums;
using PartsHoleLib.Interfaces;

namespace PartsHoleLib;

public class UserModel : IModel
{
   [BsonId]
   [BsonRepresentation(BsonType.ObjectId)]
   public string Id { get; set; } = null!;
   public string UserName { get; set; } = null!;
   public string AuthID { get; set; } = null!;
   public string? Email { get; set; }

   [BsonRepresentation(BsonType.ObjectId)]
   public List<string> Parts { get; set; } = null!;
   [BsonRepresentation(BsonType.ObjectId)]
   public List<string> Invoices { get; set; } = null!;
   [BsonRepresentation(BsonType.ObjectId)]
   public List<string> Bins { get; set; } = null!;
   [BsonRepresentation(BsonType.ObjectId)]
   public List<string> PartNumbers { get; set; } = null!;

}
