using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
    public class Group:EntityBase
    {
        public Group()
        {
            IsDeleted = false;
        }

        [Key]
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<Ranges> Ranges { get; set; }

       
    }
}
