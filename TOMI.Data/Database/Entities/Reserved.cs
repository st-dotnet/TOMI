using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
   public class Reserved:EntityBase
    {
        [Key]
        public Guid Id { get; set; }
        public string Store { get; set; }
        public string Code { get; set; }
        public string Quantity { get; set; }
        public string Filler { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTimeOffset? StockDate { get; set; }

    }
}
