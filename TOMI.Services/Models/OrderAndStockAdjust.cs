using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Services.Models
{
   public class OrderAndStockAdjust
    {
        public string Store { get; set; }
        public DateTimeOffset? InventoryDate { get; set; }
        public string SalePrice { get; set; }
        public int? Quantity { get; set; }
    }
}
