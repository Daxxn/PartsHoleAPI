using MongoDB.Driver;

using PartsHoleAPI.DBServices.Interfaces;

using PartsHoleLib;
using PartsHoleLib.Interfaces;

namespace PartsHoleAPI.DBServices;

public interface IPartNumberService : ICollectionService<IPartNumber>
{

}