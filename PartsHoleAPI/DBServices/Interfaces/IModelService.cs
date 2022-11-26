using MongoDB.Driver;
using MongoDB.Bson;

namespace PartsHoleAPI.DBServices.Interfaces;

public interface IModelService<TModel>
{
    IMongoCollection<TModel> Collection { get; init; }

    /// <summary>
    /// Gets a <see cref="TModel"/> from the database.
    /// </summary>
    /// <param name="id"><see cref="ObjectId"/> to find</param>
    /// <returns>Found <see cref="TModel"/>, <seealso cref="null"/> if not found.</returns>
    Task<TModel?> GetFromDatabaseAsync(string id);

    /// <summary>
    /// Adds a <see cref="TModel"/> to the database.
    /// </summary>
    /// <param name="data"><see cref="TModel"/> to add</param>
    /// <returns>True if successfull.</returns>
    Task<bool> AddToDatabaseAsync(TModel data);

    /// <summary>
    /// Updates a <see cref="TModel"/> in the database.
    /// </summary>
    /// <param name="id"><see cref="ObjectId"/> to update</param>
    /// <param name="data"><see cref="TModel"/> to update</param>
    /// <returns>True if successfull.</returns>
    Task<bool> UpdateDatabaseAsync(string id, TModel data);

    /// <summary>
    /// Deletes a model from the database.
    /// </summary>
    /// <param name="id"><see cref="ObjectId"/> to update</param>
    /// <returns>True if successfull.</returns>
    Task<bool> DeleteFromDatabaseAsync(string id);
}
