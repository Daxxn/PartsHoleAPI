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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PartsHoleModelLibrary;
using MongoDB.Bson;
using PartsHoleRestLibrary.Responses;
using Swashbuckle.AspNetCore.Swagger;

namespace PartsHoleAPI
{
   public class Program
   {
      public static void Main(string[] args)
      {
         var builder = WebApplication.CreateBuilder(args);

         #region Add config variables to Services
         builder.Services.Configure<DatabaseSettings>(
            builder.Configuration.GetSection("Database"));
         builder.Services.Configure<Auth0Settings>(
            builder.Configuration.GetSection("Auth0"));
         #endregion

         #region Add Logging to Services
         builder.Services.AddLogging((loggerConfig) =>
         {
#if DEBUG
            loggerConfig.AddConsole();
            loggerConfig.AddDebug();
#endif
         });
         #endregion

         #region Add Auth0 Authentication to Services
         //var auth0 = builder.Configuration.Get<Auth0Settings>();

         //builder.Services.AddAuthentication(opt =>
         //{
         //   opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
         //   opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
         //}).AddJwtBearer(opt =>
         //{
         //   opt.Authority = builder.Configuration["Auth0:Authority"];
         //   opt.Audience = builder.Configuration["Auth0:Audience"];
         //});
         #endregion

         builder.Services.AddControllers();
         // OpenAPI : https://aka.ms/aspnetcore/swashbuckle
         builder.Services.AddEndpointsApiExplorer();
         builder.Services.AddSwaggerGen();

         builder.Services.AddRouting((options) => options.LowercaseUrls = true);

         #region Configure MongoDB Serializers
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
         BsonSerializer.RegisterSerializer(
            new ImpliedImplementationInterfaceSerializer<IUserData, UserData>(
               BsonSerializer.LookupSerializer<UserData>()));
         #endregion

         #region Register models
         builder.Services.AddScoped<IUserModel, UserModel>();
         builder.Services.AddScoped<IUserData, UserData>();
         builder.Services.AddScoped<IPartModel, PartModel>();
         builder.Services.AddScoped<IBinModel, BinModel>();
         builder.Services.AddScoped<IInvoiceModel, InvoiceModel>();
         #endregion

         #region Register MongoDB Collection Wrappers
         builder.Services.AddSingleton<IUserCollection, UserCollection>();
         builder.Services.AddSingleton<ICollectionService<IPartModel>, CollectionService<IPartModel>>();
         builder.Services.AddSingleton<ICollectionService<IBinModel>, CollectionService<IBinModel>>();
         builder.Services.AddSingleton<ICollectionService<IInvoiceModel>, CollectionService<IInvoiceModel>>();
         #endregion

         var app = builder.Build();

         #region Configure HTTP request pipeline.
         if (app.Environment.IsDevelopment())
         {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors((cors) =>
            {
               // Going to have to figure out What kind of CORS im gonna need.
               cors.WithOrigins(
                  "https://localhost:3000"
               );
            });
         }

         app.UseHttpsRedirection();
         app.UseAuthorization();
         app.MapControllers();
         app.Run();
         #endregion
      }
   }
}