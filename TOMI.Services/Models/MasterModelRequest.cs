using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Services.Models
{
   public class MasterModelRequest
    {
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTime? StockDate { get; set; }
    }
}
