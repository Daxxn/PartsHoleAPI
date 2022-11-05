namespace PartsHoleRestLibrary.Responses;

public interface IAPIResponse<T>
{
   string Method { get; init; }
   T? Body { get; init; }
   string Message { get; init; }
}
