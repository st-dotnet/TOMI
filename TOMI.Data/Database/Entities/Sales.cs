using System;
using System.ComponentModel.DataAnnotations;

namespace TOMI.Data.Database.Entities
{
    public class Sales:EntityBase
    {
        [Key]
        public Guid Id { get; set; }
        public string Store { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string Total { get; set; }
        public string HUA { get; set; }
        public string Area { get; set; }
        public string Family { get; set; }
        public string Lineal { get; set; }
        public string Metro { get; set; }
        public string Department { get; set; }
        public string Departmentname { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTimeOffset? StockDate { get; set; }




    }
}
