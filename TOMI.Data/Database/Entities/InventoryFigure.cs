using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
    public class InventoryFigure
    {
        [Key]
        public int Id { get; set; }
        public string StoreNumber{ get; set; }
        public DateTimeOffset? FigureDate { get; set; }
        public int Unit { get; set; }
        public string Amount { get; set; }

    }
}
