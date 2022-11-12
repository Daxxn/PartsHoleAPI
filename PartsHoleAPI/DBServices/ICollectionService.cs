using MongoDB.Driver;
using MongoDB.Bson;

namespace PartsHoleAPI.DBServices;

public interface ICollectionService<T> : IModelService<T>
{
   /// <summary>
   /// Sends the find request to the database.
   /// </summary>
   /// <param name="ids"><see cref="ObjectId"/>s to find</param>
   /// <returns><see cref="List{T}"/> of found models.</returns>
   Task<IEnumerable<T>?> GetFromDatabaseAsync(string[] ids);

   /// <summary>
   /// Adds <paramref name="data"/> models to the database.
   /// </summary>
   /// <param name="data">Models to create</param>
   /// <returns><see cref="List{T}"/> of <see cref="bool"/> representing successfull model creation.</returns>
   Task<IEnumerable<bool>?> AddToDatabaseAsync(IEnumerable<T> data);

   /// <summary>
   /// Updates <paramref name="data"/> models in the database.
   /// If model doesnt exist, creates new model.
   /// </summary>
   /// <param name="data">Models to update</param>
   /// <returns><see cref="List{T}"/> of <see cref="bool"/> representing successfull model creation.</returns>
   Task<IEnumerable<bool>?> UpdateDatabaseAsync(IEnumerable<T> data);

   /// <summary>
   /// Removes models from the database by <see cref="ObjectId"/>.
   /// </summary>
   /// <param name="ids"><see cref="ObjectId"/>s to remove</param>
   /// <returns>Number of removed models.</returns>
   Task<int> DeleteFromDatabaseAsync(string[] ids);
}
