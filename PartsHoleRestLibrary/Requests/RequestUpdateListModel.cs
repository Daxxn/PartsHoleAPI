using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsHoleRestLibrary.Requests;

 public class RequestUpdateListModel
 {
   public string? UserId { get; set; }
   public string? ModelId { get; set; }
   public int PropId { get; set; } = -1;
}
