using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
   public class Departments:EntityBase
    {
        [Key]
        public Guid Id { get; set; }
        public string Division { get; set; }
        public string DivisionName { get; set; }
        public string Department { get; set; }
        public string DepartmentName { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTimeOffset? StockDate { get; set; }
    }
}
