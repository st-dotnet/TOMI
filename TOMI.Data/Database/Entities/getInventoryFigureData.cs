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
        public int Amount { get; set; }
        public int Qty { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
