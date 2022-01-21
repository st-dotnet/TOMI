using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;

namespace TOMI.Services.Models
{
   public class StockAdjustmentModel
    {
        public Guid? Id { get; set; }
        public Guid? Rec { get; set; }
        public string Term { get; set; }
        public int? Dload { get; set; }
        public int? Tag { get; set; }
        public int? Shelf { get; set; }
        public string Barcode { get; set; }
        public string SKU { get; set; }
        public byte NOF { get; set; }
        public int? Department { get; set; }
        public int? Quantity { get; set; }
        public byte Isdeleted { get; set; }
    }

    public class StockAdjustmentResponse:BaseResponse
    {
        public StockAdjustment Adjustment { get; set; }
    }
}
