using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Services.Models
{
    public class Inventario
    {
        public int Id { get; set; }
        public int Zona { get; set; }
        public string Tag { get; set; }
        public int Consecutive { get; set; }
        public int Code { get; set; }
        public int Department { get; set; }
        public int Qty { get; set; }
        public int Price { get; set; }
        public byte Price_Indicator { get; set; }
        public DateTimeOffset Date { get; set; }
        public DateTimeOffset Time { get; set; }
        public int Filler { get; set; }
    }
}
