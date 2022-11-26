namespace PartsHoleRestLibrary.Exceptions;

public class ModelAlreadyExistsException : Exception
{
   public string ModelID { get; init; }
   public ModelAlreadyExistsException(string modelID, string? message) : base(message) => ModelID = modelID;
}
