namespace PartsHoleRestLibrary.Responses;

public class APIResponse<T> : IAPIResponse<T>
{
   #region Local Props
   public string Method { get; init; }
   public T? Body { get; init; }
   public string Message { get; init; }
   #endregion

   #region Constructors
   public APIResponse(T body, string method, string message)
   {
      Method = method;
      Body = body;
      Message = message;
   }
   public APIResponse(string method, string message)
   {
      Method = method;
      Message = message;
   }
   public APIResponse(T body, string method)
   {
      Body = body;
      Method = method;
      Message = "Success";
   }
   #endregion

   #region Methods

   #endregion

   #region Full Props

   #endregion
}
