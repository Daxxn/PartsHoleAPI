using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace PartsHoleRestLibrary.Requests
{
   public class ParseFileRequestModel
   {
      public string UserId { get; set; } = null!;
      public IFormFile File { get; set; } = null!;
   }
}
