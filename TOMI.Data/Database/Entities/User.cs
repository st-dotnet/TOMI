using System;
using System.ComponentModel.DataAnnotations;

namespace TOMI.Data.Database.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid StoreId { get; set; } 

        public virtual Customer Customer { get; set; }
        public virtual Store Store { get; set; }
    }
}
