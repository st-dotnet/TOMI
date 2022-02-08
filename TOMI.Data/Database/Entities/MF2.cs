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
        [Key]
        public Guid Id { get; set; }
        public string Department { get; set; }
        public DateTime creation_time { get; set; }

    }

}
