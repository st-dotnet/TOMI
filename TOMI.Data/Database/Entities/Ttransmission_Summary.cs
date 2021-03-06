using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
  public class Ttransmission_Summary
    {
        [Key]
        public int Id { get; set; }
        public string Terminal { get; set; }
        public string EmployeeNumber { get; set; }
        public int TotalSend { get; set; }
        public int Lines { get; set; }
        public int QuantityCounted { get; set; }
        public int PriceCounted { get; set; }
        public DateTime CreationTime { get; set; }
        public int Tag { get; set; }
        public string EmployeeName { get; set; }
        public int DuplicateTag { get; set; }
    }
}
