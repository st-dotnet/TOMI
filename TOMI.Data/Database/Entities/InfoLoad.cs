using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
    public class InfoLoad:EntityBase
    {
        public InfoLoad()
        {
            DownloadedErrors = false;
        }
        [Key]
        public int Id { get; set; }
        public string Terminal { get; set; }
        public int Send { get; set; }
        public string Emp { get; set; }
        public DateTime Downloaded { get; set; }
        public int Lines { get; set; }
        public int Qty { get; set; }
        public double ExtPrice { get; set; }
        public bool DownloadedErrors { get; set; }
    }
}
