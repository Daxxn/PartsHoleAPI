using Microsoft.Extensions.Configuration;

using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

using PartsHoleAPI.DBServices;
using PartsHoleAPI.Utils;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

using SharpCompress.Common;
using static MongoDB.Driver.WriteConcern;

namespace PartsHoleAPI
{
   public class Program
   {
      public static void Main(string[] args)
      {
         var builder = WebApplication.CreateBuilder(args);

         // Add services to the container.
         builder.Services.Configure<DatabaseSettings>(
            builder.Configuration.GetSection("Database"));

         builder.Services.AddControllers();
         // OpenAPI : https://aka.ms/aspnetcore/swashbuckle
         builder.Services.AddEndpointsApiExplorer();
         builder.Services.AddSwaggerGen();

         BsonSerializer.RegisterSerializer(
            new ImpliedImplementationInterfaceSerializer<IUserModel, UserModel>(
               BsonSerializer.LookupSerializer<UserModel>()));
         BsonSerializer.RegisterSerializer(
            new ImpliedImplementationInterfaceSerializer<IPartModel, PartModel>(
               BsonSerializer.LookupSerializer<PartModel>()));
         BsonSerializer.RegisterSerializer(
            new ImpliedImplementationInterfaceSerializer<IBinModel, BinModel>(
               BsonSerializer.LookupSerializer<BinModel>()));
         BsonSerializer.RegisterSerializer(
            new ImpliedImplementationInterfaceSerializer<IInvoiceModel, InvoiceModel>(
               BsonSerializer.LookupSerializer<InvoiceModel>()));

         // I can get my models registered but they dont get resolved properly
         // in the ICollectionService interface / Collection class.
         builder.Services.AddScoped<IUserModel, UserModel>();
         builder.Services.AddScoped<IPartModel, PartModel>();
         builder.Services.AddScoped<IBinModel, BinModel>();
         builder.Services.AddScoped<IInvoiceModel, InvoiceModel>();

         builder.Services.AddSingleton<IModelService<IUserModel>, UserCollection>();
         builder.Services.AddSingleton<ICollectionService<IPartModel>, PartsCollection>();
         builder.Services.AddSingleton<ICollectionService<IBinModel>, BinCollection>();
         builder.Services.AddSingleton<ICollectionService<IInvoiceModel>, InvoiceCollection>();

         var app = builder.Build();

         // Configure the HTTP request pipeline.
         if (app.Environment.IsDevelopment())
         {
            app.UseSwagger();
            app.UseSwaggerUI();
         }

         app.UseHttpsRedirection();

         app.UseAuthorization();

         app.MapControllers();

         app.Run();
      }
   }
}