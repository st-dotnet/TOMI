using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TOMI.Data.Database.Entities
{
    public class Customer: EntityBase
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } 
        public virtual ICollection<User> Users { get; set; } 
        public virtual ICollection<Store> Stores { get; set; }  
    }
}
