using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Services.Models
{
   public class TerminalPost
    {
        public string Terminal { get; set; }
        public int Tag { get; set; }
        public int Shelf { get; set; }
        public string EmployeeNumber { get; set; }
        public int CountType { get; set; }
        public int TotalCounted { get; set; }
        public DateTime CountTime { get; set; }
        public bool Nof { get; set; }
        public bool CountedStatus { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
    }
}
