using PartsHoleLib;

namespace PartsHoleAPI.DBServices.Interfaces
{
    public interface IPartService : ICollectionService<PartModel>
    {
        Task<IEnumerable<PartModel>?> SearchForParts<T>(string prop, T match);
    }
}