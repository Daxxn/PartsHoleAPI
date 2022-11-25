namespace PartsHoleRestLibrary.Exceptions;

public class DatabaseDesyncException : Exception
{
   public IEnumerable<string>? FoundIDs { get; init; }
   public DatabaseDesyncException() { }
   public DatabaseDesyncException(IEnumerable<string> foundIDs) => FoundIDs = foundIDs;
   public DatabaseDesyncException(string? message) : base(message) { }
   public DatabaseDesyncException(string? message, IEnumerable<string> foundIDs) : base(message) => FoundIDs = foundIDs;
}
