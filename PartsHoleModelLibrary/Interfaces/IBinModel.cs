using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace PartsHoleLib.Interfaces
{
   public interface IBinModel : IModel
   {
      int Horizontal { get; set; }
      bool IsBook { get; set; }
      string Name { get; set; }
      int Vertical { get; set; }
   }
}