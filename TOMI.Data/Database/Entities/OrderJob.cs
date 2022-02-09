using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TOMI.Data.Database.Entities
{
    public class OrderJob: EntityBase
    {

        [Key]
        public Guid Id { get; set; }
        public string SKU { get; set; }
        public string Code { get; set; }
        public string Department { get; set; }
        public string Description { get; set; }
        public string SalePrice { get; set; }
        public string PriceWithoutTaxes { get; set; }
        public string Store { get; set; }
        public string Category { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTimeOffset? StockDate { get; set; }
        public virtual ICollection<StockAdjustment> StockAdjustment { get; set; }
       // public virtual ICollection<MF2> MF2 { get; set; }
    }
}


