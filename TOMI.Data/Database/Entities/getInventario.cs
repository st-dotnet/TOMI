using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
   public class getInventario
    {
        [Key]
        public Guid Id { get; set; }
        public string Store { get; set; }
        public string Zona { get; set; }
        public int Tag { get; set; }
        public string Consecutive { get; set; }
         public string Code { get; set; }
          public int Department { get; set; }
          public int Qty { get; set; }
          public string Price { get; set; }
         public string Price_Indicator { get; set; }
         public string Date { get; set; }
         public string Time { get; set; }
       // public int Filler { get; set; }
       // public byte NOF { get; set; }
    }
}
