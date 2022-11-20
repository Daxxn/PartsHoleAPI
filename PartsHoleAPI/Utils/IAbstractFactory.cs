namespace PartsHoleAPI.Utils
{
    public interface IAbstractFactory<T>
    {
        T Create();
    }
}