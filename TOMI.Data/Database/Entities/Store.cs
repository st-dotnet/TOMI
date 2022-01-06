using System;
using System.ComponentModel.DataAnnotations;

namespace TOMI.Data.Database.Entities
{
    public class Store
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual User User { get; set; }
    }
}
