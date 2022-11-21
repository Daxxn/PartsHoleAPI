using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsHoleRestLibrary.Requests
{
   public class PartNumberRequestModel
   {
      public string UserId { get; set; } = null!;
      public uint? Type { get; set; }
      public uint? SubType { get; set; }
   }
}
