using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Services.Models
{
  public class DeptAndStockAdjust
    {
        public int? Department { get; set; }
        public string DepartmentName { get; set; }
        public int? Quantity { get; set; }
        public string SalePrice { get; set; }
        public string PriceWithoutTaxes { get; set; }
    }
}
