using System;
using System.ComponentModel.DataAnnotations;

namespace TOMI.Data.Database.Entities
{
    public class Ranges : EntityBase
    {
        public Ranges()
        {
            IsDeleted = false;
        }
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? GroupId { get; set; }
        public int? TagFrom { get; set; }
        public int? TagTo { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTimeOffset? StockDate { get; set; }
        public virtual Group Group { get; set; }
        public bool IsDeleted { get; set; }

    }
}
