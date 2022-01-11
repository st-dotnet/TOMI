using System;
using System.ComponentModel.DataAnnotations;

namespace TOMI.Data.Database.Entities
{
    public class StoreDetails:EntityBase
    {
        [Key]
        public Guid Id { get; set; }
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

        public Guid customerId { get; set; }

         public DateTime? StockDate { get; set; }
    }
}
