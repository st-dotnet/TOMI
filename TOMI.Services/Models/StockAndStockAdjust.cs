using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Services.Models
{
 public  class StockAndStockAdjust
    {
        public string SKU { get; set; }
        public string Description { get; set; }
        public string SOH { get; set; }
        public string PrecVtaNorm { get; set; }
        public int Quantity { get; set; }
        public string Barcode { get; set; }
        public int Tag { get; set; }
        public Guid Id { get; set; }
    }
}
