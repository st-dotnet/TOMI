using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
    public class DwnErrors: EntityBase
    {
        [Key]
        public int id { get; set; }
        public int Tag { get; set; }
        public string ErrorMessage { get; set; }
        public string Term { get; set; }
        public string EmpNo { get; set; }
        public int Lines { get; set; }
        public int Qty { get; set; }
        public double ExtPrice { get; set; }
    }
}
