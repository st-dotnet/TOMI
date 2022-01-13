using System;
using System.ComponentModel.DataAnnotations;

namespace TOMI.Data.Database.Entities
{
    public class Stocks: EntityBase
    {
        [Key]
        public Guid Id { get; set; }
        public string SKU { get; set; }
        public string Barcode { get; set; }
        public string RetailPrice { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }
        public string Blank { get; set; }
        public string OHQuantity { get; set; }
        public string Unity { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTimeOffset? StockDate { get; set; }

    }
}
