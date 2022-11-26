using MongoDB.Driver;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

using PartsHoleRestLibrary.Requests;

namespace PartsHoleAPI.DBServices.Interfaces;

public interface IPartNumberService : ICollectionService<PartNumber>
{
    Task<PartNumber> GeneratePartNumberAsync(PartNumberRequestModel requestData);
}