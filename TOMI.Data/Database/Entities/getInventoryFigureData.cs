using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
    public class getInventoryFigureData
    {
        [Key]
        public Guid Id { get; set; }
        public string StoreName { get; set; }
        public string Amount { get; set; }
        public string Qty { get; set; }
       
    }
}
