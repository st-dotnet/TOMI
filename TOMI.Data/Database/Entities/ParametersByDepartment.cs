using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
    public class ParametersByDepartment : EntityBase
    {
        [Key]
        public Guid Id { get; set; }
        public string Department { get; set; }
        public string Quantity { get; set; }
        public string Pesos { get; set; }
        public string PercentageInPieces { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTime? StockDate { get; set; }
    }
}
