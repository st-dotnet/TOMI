using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
    public class getMarbete
    {
        [Key]
        public  Guid Id { get; set; }

        public string Zona { get; set; }
        public string Department { get; set; }
        public string Tag { get; set; }
     
    }
}
