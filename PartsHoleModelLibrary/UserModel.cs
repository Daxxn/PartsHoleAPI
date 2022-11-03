using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using PartsHoleLib.Interfaces;

namespace PartsHoleLib
{
   public class UserModel : IUserModel
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
   }
}
