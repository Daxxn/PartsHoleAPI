using CSVParserLibrary;

using ExcelParserLibrary;

using Microsoft.AspNetCore.Authentication.JwtBearer;

using OfficeOpenXml;

using PartsHoleAPI.DBServices;
using PartsHoleAPI.DBServices.Interfaces;
using PartsHoleAPI.Utils;

using PartsHoleLib;

namespace PartsHoleAPI;

public class Program
{
   public static void Main(string[] args)
   {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

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
         loggerConfig.AddConsoleFormatter<CustomConsoleFormatter, CustomConsoleOptions>();
         loggerConfig.AddConsole((opt) =>
         {
            opt.FormatterName = "Custom";
         });
         loggerConfig.AddDebug();
#endif
      });
      #endregion

      #region Add Auth0 Authentication to Services
      RegisterAuth0(builder);
      #endregion

      builder.Services.AddControllers();
      // OpenAPI : https://aka.ms/aspnetcore/swashbuckle
      builder.Services.AddEndpointsApiExplorer();
      builder.Services.AddSwaggerGen();

      builder.Services.AddRouting((options) => options.LowercaseUrls = true);

      RegisterModels(builder.Services);
      RegisterCollectionServices(builder.Services);

      var app = builder.Build();

      #region Configure HTTP request pipeline.
      if (app.Environment.IsDevelopment())
      {
         app.UseHttpLogging();
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
      app.UseAuthentication();
      app.UseAuthorization();
      app.MapControllers();
      app.Run();
      #endregion
   }

   private static void RegisterModels(IServiceCollection Services)
   {
      Services.AddTransient<ICSVParserOptions, CSVParserOptions>();
      Services.AddAbstractFactory<ICSVParser, CSVParser>();
      Services.AddAbstractFactory<IExcelParser, ExcelParser>();
   }

   private static void RegisterCollectionServices(IServiceCollection Services)
   {
      Services.AddSingleton<IUserService, UserService>();
      Services.AddSingleton<IPartService, PartService>();
      Services.AddSingleton<ICollectionService<BinModel>, CollectionService<BinModel>>();
      Services.AddSingleton<IInvoiceService, InvoiceService>();
      Services.AddSingleton<IPartNumberService, PartNumberService>();
      Services.AddSingleton<IMouserParseService, MouserParseService>();
   }

   private static void RegisterAuth0(WebApplicationBuilder builder)
   {
      var auth0 = builder.Configuration.Get<Auth0Settings>();

      builder.Services.AddAuthentication(opt =>
      {
         opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
         opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      }).AddJwtBearer(opt =>
      {
         opt.Authority = builder.Configuration["Auth0:Authority"];
         opt.Audience = builder.Configuration["Auth0:Audience"];
      });
   }
}