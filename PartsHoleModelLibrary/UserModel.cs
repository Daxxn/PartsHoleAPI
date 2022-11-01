using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using PartsHoleAPI.Models.Interfaces;

namespace PartsHoleAPI.Models
{
   public class UserModel : IUserModel
   {
      [BsonId]
      [BsonRepresentation(BsonType.ObjectId)]
      public string Id { get; set; } = null!;
      public string Username { get; set; } = null!;
      public string AuthID { get; set; } = null!;

      public List<IPartModel> Parts { get; set; } = null!;
      public List<IInvoiceModel> Invoices { get; set; } = null!;
      // Going to add later. Figuring out the foundation of the api is more important right now.
      //public PassivesCollection Passives { get; set; }
   }
}
