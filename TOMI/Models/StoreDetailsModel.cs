using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TOMI.Web.Models
{
    public class StoreDetailsModel
    {
        public int? Id { get; set; }
        [Index(0)]
        public string Store { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Time { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? Total { get; set; }
        public string HUA { get; set; }
        public string Area { get; set; }
        public string Family { get; set; }
        public string Lineal { get; set; }
        public string Metro { get; set; }
        public string Department { get; set; }
        public string Departmentname { get; set; }
    }
}
