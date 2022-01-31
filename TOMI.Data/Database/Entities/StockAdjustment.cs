using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
   public class StockAdjustment:EntityBase
    {
        public StockAdjustment()
        {
            Isdeleted = false;
        }

        [Key]
        public Guid Id { get; set; }
        public int? Empno { get; set; }
        public int? Rec { get; set; }
        public string Term { get; set; }
        public int? Dload { get; set; }
        public int? Tag { get; set; }
        public int? Shelf { get; set; }
        public string Barcode { get; set; }
        public Guid SKU { get; set; }
        public byte NOF { get; set; }
        public int? Department { get; set; }
        public int? Quantity { get; set; }
        public bool Isdeleted { get; set; }
        public virtual Master Master { get; set; }
    }
}
