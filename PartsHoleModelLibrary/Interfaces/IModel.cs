﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace PartsHoleAPI.Models.Interfaces
{
   public interface IModel
   {
      [BsonId]
      [BsonRepresentation(BsonType.ObjectId)]
      string Id { get; set; }
   }
}
