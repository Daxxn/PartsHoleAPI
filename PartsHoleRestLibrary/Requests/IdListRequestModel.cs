namespace PartsHoleRestLibrary.Requests;

public class IdListRequestModel
{
   public IEnumerable<string> IDs { get; set; } = null!;

   public IdListRequestModel() { }
   public IdListRequestModel(IEnumerable<string> ids)
   {
      IDs = ids;
   }
}
