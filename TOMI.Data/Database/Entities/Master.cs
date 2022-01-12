using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
    public class Master
    {
        [Key]
        public Guid Id { get; set; }
        public string Store { get; set; }
        public string SKU { get; set; }
        public string Filler { get; set; }
        public string Blank { get; set; }
        public string RecordId { get; set; }
        public string Quantity { get; set; }
        public string Unity { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTime? StockDate { get; set; }




    }
}
