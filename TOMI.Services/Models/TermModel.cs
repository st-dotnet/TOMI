using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;

namespace TOMI.Services.Models
{
 public  class TermModel
    {
        public OperationType Operation { get; set; }
        public string InventoryKey { get; set; }

    }
}
