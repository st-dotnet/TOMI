using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
    public class MF2 : EntityBase
    {
        
        public Guid Id { get; set; }
        [Key]
        public string Department { get; set; }
        public DateTimeOffset creation_time { get; set; }

       public virtual ICollection<MF1> MF1 { get; set; }

      

    }

}
