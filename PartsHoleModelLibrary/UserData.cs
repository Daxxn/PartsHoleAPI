using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PartsHoleLib.Interfaces;

namespace PartsHoleModelLibrary
{
   public class UserData : IUserData
   {
      public List<IPartModel> Parts { get; set; } = null!;
      public List<IInvoiceModel> Invoices { get; set; } = null!;
   }
}
