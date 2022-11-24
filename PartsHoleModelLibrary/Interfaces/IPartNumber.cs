using PartsHoleLib;

namespace PartsHoleLib.Interfaces
{
    public interface IPartNumber
    {
        uint Category { get; set; }
        uint FullCategory { get; }
        uint ID { get; set; }
        uint SubCategory { get; set; }

        int CompareTo(PartNumber? other);
        bool Equals(object? obj);
        bool Equals(PartNumber? other);
        int GetHashCode();
        string ToString();
    }
}