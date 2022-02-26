using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Services.Models
{
    public  class ReservedAndOrderModel
    {
        public string barCode { get; set; }
        public Guid? JobOrderId { get; set; }
        public string Tag { get; set; }
        public string Shelf { get; set; }
        public Guid SKU { get; set; }
        public bool NOF { get; set; }
        public double Qty { get; set; }

        public bool Isdeleted { get; set; }
        public bool Isactive { get; set; }

        public DateTimeOffset? Date { get; set; }
    }
}
