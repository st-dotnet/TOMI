using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
    public class BillingReport
    {
        [Key]
        public int Id { get; set; }
        public string Qty { get; set; }
        public string ExtCost { get; set; }
        public string ExtPrice { get; set; }
        public string Lines { get; set; }
        public string GroupName { get; set; }
    }
}
