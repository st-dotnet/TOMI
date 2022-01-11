using System;
using CsvHelper.Configuration.Attributes;

namespace TOMI.Services.Models
{
    public class StoreDetailsResponse
    {
        [Index(0)]
        public string Store { get; set; }
        [Index(1)]
        public string Date { get; set; }
        [Index(2)]
        public string Time { get; set; }
        [Index(3)]
        public string SKU { get; set; }
        [Index(4)]
        public string Description { get; set; }
        [Index(5)]
        public string Quantity { get; set; }
        [Index(6)]
        public string Price { get; set; }
        [Index(7)]
        public string Total { get; set; }
        [Index(8)]
        public string HUA { get; set; }
        [Index(9)]
        public string Area { get; set; }
        [Index(10)]
        public string Family { get; set; }
        [Index(11)]
        public string Lineal { get; set; }
        [Index(12)]
        public string Metro { get; set; }
        [Index(13)]
        public string Department { get; set; }
        [Index(14)]
        public string Departmentname { get; set; }
        
      

    }


   
}
