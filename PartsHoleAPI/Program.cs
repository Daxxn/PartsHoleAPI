using Microsoft.Extensions.Configuration;

using MongoDB.Driver;

using PartsHoleAPI.Collections;
using PartsHoleAPI.Models;
using PartsHoleAPI.Models.Interfaces;

namespace PartsHoleAPI
{
   public class Program
   {
      public static void Main(string[] args)
      {
         var builder = WebApplication.CreateBuilder(args);

         // Database testing...
         TestDBConnection(builder);

         // Add services to the container.
         //builder.Services.Configure<DatabaseSettings>(
         //    builder.Configuration.GetSection("DatabaseSettings"));
         //var moviesConfig = builder.Configuration.GetSection("Mongo")

         builder.Services.Configure<DatabaseSettings>(
            builder.Configuration.GetSection("Database"));


         builder.Services.AddControllers();
         // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
         builder.Services.AddEndpointsApiExplorer();
         builder.Services.AddSwaggerGen();

         builder.Services.AddSingleton<UserCollection>();
         builder.Services.AddSingleton<PartsCollection>();

         builder.Services.AddTransient<IPartModel, PartModel>();
         builder.Services.AddTransient<IUserModel, UserModel>();

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

      private static async Task TestDBConnection(WebApplicationBuilder builder)
      {
         MongoClient client = new("mongodb+srv://admin:VcXAvL1ki0mphGKR@partsinventorydev.ljk92th.mongodb.net/?retryWrites=true&w=majority");
         var dbs = await client.ListDatabaseNames().ToListAsync();

         if (dbs is null)
         {
            Console.WriteLine("Unable to find DB...");
         }
      }
   }
}