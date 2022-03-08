using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
    public class spCodeNotfoundReport
    {
        [Key]
        public Guid Id { get; set; }
        public string RecNo { get; set; }
        public string Shelf { get; set; }
        public string Department { get; set; }
        public string Tag { get; set; }
        public string Barcode { get; set; }
        public string SalePrice { get; set; }
        public string ExtPrice { get; set; }
        public string Quantity { get; set; }
        public string Total_Ext_Price { get; set; }
        public string Total_Quantity { get; set; }

    }
}
