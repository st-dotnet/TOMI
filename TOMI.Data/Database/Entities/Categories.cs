using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
   public class Categories:EntityBase
    {
        public int Id { get; set; }
        public string Division { get; set; }
        public string DivisionName { get; set; }
        public string Category { get; set; }
        public string CategoryName { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTime? StockDate { get; set; }
    }
}
