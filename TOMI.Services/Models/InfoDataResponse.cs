using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace TOMI.Services.Models
{
    public class InfoDataResponse 
    {
        [Index(0)]
        public string Terminal { get; set; }
        [Index(1)]
        public int Send { get; set; }
        [Index(2)]
        public string Emp { get; set; }
        [Index(3)]
        public DateTime Downloaded { get; set; }
        [Index(4)]
        public int Lines { get; set; }
        [Index(5)]
        public int Qty { get; set; }
        [Index(6)]
        public double ExtPrice { get; set; }
        [Index(7)]
        public bool DownloadedErrors { get; set; }
    }
}
