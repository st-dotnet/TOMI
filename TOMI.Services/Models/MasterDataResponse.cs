using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace TOMI.Services.Models
{
    public class MasterDataResponse
    {
        [Index(0)]
        public string Store { get; set; }
        [Index(1)]
        public string SKU { get; set; }
        [Index(2)]
        public string Filler { get; set; }
        [Index(3)]
        public string Blank { get; set; }
        [Index(4)]
        public string RecordId { get; set; }
        [Index(5)]
        public string Quantity { get; set; }
        [Index(6)]
        public string Unity { get; set; }
      
        
    }
}
