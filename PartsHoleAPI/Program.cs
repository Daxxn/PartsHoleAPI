using MongoDB.Driver;

using PartsHoleAPI.Collections;
using PartsHoleAPI.Models;

namespace PartsHoleAPI
{
   public class Program
   {
      public static void Main(string[] args)
      {
         var builder = WebApplication.CreateBuilder(args);

         // Add services to the container.

         ////MongoClient client = new(builder.Configuration["Mongo:ConnectionString"]);

         // Add services to the container.
         builder.Services.Configure<DatabaseSettings>(
             builder.Configuration.GetSection("DatabaseSettings"));

         builder.Services.AddControllers();
         // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
         builder.Services.AddEndpointsApiExplorer();
         builder.Services.AddSwaggerGen();

         builder.Services.AddSingleton<UserCollection>();
         builder.Services.AddSingleton<PartsCollection>();

         //var clientService = builder.Services.AddSingleton<IMongoClient>(client);


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