using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using PartsHoleLib.Enums;

namespace PartsHoleLib.Interfaces;

public interface IUserModel : IModel
{
   string UserName { get; set; }
   string AuthID { get; set; }
   string? Email { get; set; }

   [BsonRepresentation(BsonType.ObjectId)]
   List<string> Invoices { get; set; }
   [BsonRepresentation(BsonType.ObjectId)]
   List<string> Parts { get; set; }
   [BsonRepresentation(BsonType.ObjectId)]
   List<string> Bins { get; set; }
}