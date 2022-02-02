using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
   public class Stock:EntityBase
    {
        [Key]
        public Guid Id { get; set; }
        public string Store { get; set; }
        public string SKU { get; set; }
        public string Departament { get; set; }
        public string Description { get; set; }
        public string PrecVtaNorm { get; set; }
        public string PrecVtaNorm_SImpto { get; set; }
        public string SOH { get; set; }
        public string Category { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTimeOffset? StockDate { get; set; }



    }
}
