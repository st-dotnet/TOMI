using System;
using System.ComponentModel.DataAnnotations;

namespace TOMI.Data.Database.Entities
{
    public class User : EntityBase
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? StoreId { get; set; } 
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string EmployeeNumber { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Store Store { get; set; }
    }
}
