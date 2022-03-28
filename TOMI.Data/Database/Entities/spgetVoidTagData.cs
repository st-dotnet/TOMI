using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
   public class spgetVoidTagData
    {
        [Key]
        public Guid Id { get; set; }
        public string TagStatus { get; set; }
        public int? TagFrom { get; set; }
        public int? TagTo { get; set; }
        public string Name { get; set; }
    }
}
