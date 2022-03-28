using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TOMI.Data.Database.Entities
{
    public class Store : EntityBase
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public string StoreAddress { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Terminal_Smf> MF1 { get; set; }
    }
}
