using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
    public class MTerminalSummary
    {
        [Key]
        public Guid Id { get; set; }
        public int tag { get; set; }
        public int shelf { get; set; }
        public string Employee_Number { get; set; }

        public string Terminal { get; set; }
        public double Sale_Price { get; set; }
        public int Qty { get; set; }
      
    }
}
