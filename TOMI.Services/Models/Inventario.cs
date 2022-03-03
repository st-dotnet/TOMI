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
        public string Store { get; set; }
        public string Zona { get; set; }
        public int Tag { get; set; }
        public int Consecutive { get; set; }
        public string Code { get; set; }
        public int Department { get; set; }
        public int Qty { get; set; }
        public string Price { get; set; }
        public byte Price_Indicator { get; set; }
        public DateTimeOffset Date { get; set; }
        public DateTimeOffset Time { get; set; }
        public int Filler { get; set; }

        public byte NOF { get; set; }
    }
}
