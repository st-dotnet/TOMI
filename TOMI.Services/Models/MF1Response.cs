using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Services.Models
{
    public class MF1Response
    {
        public List<TerminalSmf> TerminalSmf { get; set; }
    }
    public class TerminalSmf
    {
        public Guid Customer { get; set; }
        public string Terminal { get; set; }
        public string Store { get; set; } 
        public string EmployeeNumber { get; set; }
        public DateTimeOffset InventoryDate { get; set; }
        public string Department { get; set; }
        public string Code { get; set; }
        public decimal SalePrice { get; set; }
        public int Tag { get; set; }
        public int Shelf { get; set; }
        public int Operation { get; set; }
        public string InventoryKey { get; set; }
        public int CountType { get; set; }
        public int TotalCounted { get; set; }
        public DateTime CountTime { get; set; }
        public bool Nof { get; set; }
        public bool CountedStatus { get; set; }

        


    }
}
