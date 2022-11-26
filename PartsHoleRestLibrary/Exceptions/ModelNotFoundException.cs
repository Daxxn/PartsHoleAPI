using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PartsHoleRestLibrary.Exceptions;

public class ModelNotFoundException : Exception
{
   public string ModelName { get; set; } = null!;
   public ModelNotFoundException(string modelName) => ModelName = modelName;
   public ModelNotFoundException(string modelName, string? message) : base(message)
      => ModelName = modelName;
   public ModelNotFoundException(string modelName, string? message, Exception? innerException)
      : base(message, innerException)
      => ModelName = modelName;
}
