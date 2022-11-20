﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PartsHoleLib.Interfaces;
using PartsHoleLib.Enums;

namespace PartsHoleLib
{
   public class InvoiceModel : IInvoiceModel
   {
      [BsonId]
      [BsonRepresentation(BsonType.ObjectId)]
      public string _id { get; set; } = null!;
      public List<DigiKeyPartModel> Parts { get; set; } = null!;
      public int? SupplierType { get; set; }
      public string? Path { get; set; }
      public int OrderNumber { get; set; }
      public decimal SubTotal { get; set; }
      public bool IsAddedToParts { get; set; }

      public InvoiceModel()
      {
         _id = ObjectId.GenerateNewId().ToString();
      }
   }
}
