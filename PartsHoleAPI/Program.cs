using Microsoft.Extensions.Configuration;

using MongoDB.Driver;

using PartsHoleAPI.DBServices;
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

         // Add services to the container.
         builder.Services.Configure<DatabaseSettings>(
            builder.Configuration.GetSection("Database"));

         builder.Services.AddControllers();
         // OpenAPI : https://aka.ms/aspnetcore/swashbuckle
         builder.Services.AddEndpointsApiExplorer();
         builder.Services.AddSwaggerGen();

         // I can get my models registered but they dont get resolved properly
         // in the ICollectionService interface / Collection class.
         //builder.Services.AddTransient<IUserModel, UserModel>();
         //builder.Services.AddTransient<IPartModel, PartModel>();
         //builder.Services.AddTransient<IBinModel, BinModel>();
         //builder.Services.AddTransient<IInvoiceModel, InvoiceModel>();

         builder.Services.AddSingleton<IModelService<UserModel>, UserCollection>();
         builder.Services.AddSingleton<ICollectionService<PartModel>, PartsCollection>();
         builder.Services.AddSingleton<ICollectionService<BinModel>, BinCollection>();

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