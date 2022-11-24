using CSVParserLibrary;

using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

using PartsHoleAPI.DBServices;
using PartsHoleAPI.DBServices.Interfaces;
using PartsHoleAPI.Utils;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

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
         BsonSerializer.RegisterSerializer(
            new ImpliedImplementationInterfaceSerializer<IPartNumber, PartNumber>(
               BsonSerializer.LookupSerializer<PartNumber>()));
         #endregion

         #region Register models
         builder.Services.AddScoped<IUserModel, UserModel>();
         builder.Services.AddScoped<IUserData, UserData>();
         builder.Services.AddScoped<IPartModel, PartModel>();
         builder.Services.AddScoped<IBinModel, BinModel>();
         builder.Services.AddScoped<IInvoiceModel, InvoiceModel>();
         builder.Services.AddScoped<IPartNumber, PartNumber>();
         #endregion

         #region Register Transient Models
         builder.Services.AddTransient<ICSVParserOptions, CSVParserOptions>();
         builder.Services.AddAbstractFactory<ICSVParser, CSVParser>();
         #endregion

         #region Register endpoint Services
         builder.Services.AddSingleton<IUserService, UserService>();
         builder.Services.AddSingleton<ICollectionService<IPartModel>, CollectionService<IPartModel>>();
         builder.Services.AddSingleton<ICollectionService<IBinModel>, CollectionService<IBinModel>>();
         builder.Services.AddSingleton<IInvoiceService, InvoiceService>();
         builder.Services.AddSingleton<IPartNumberService, PartNumberService>();
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