using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Services.Models
{
   public class StockAdjustAndOrder
    {
        public string Store { get; set; }
        public string SalePrice { get; set; }
        public int? Tag { get; set; }
        public string Barcode { get; set; }
        public int? Department { get; set; }
        public int? Quantity { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }


    }
}
