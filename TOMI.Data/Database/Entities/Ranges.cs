using System;
using System.ComponentModel.DataAnnotations;

namespace TOMI.Data.Database.Entities
{
    public class Ranges : EntityBase
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? GroupId { get; set; }
        public string TagFrom { get; set; }
        public string TagTo { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTimeOffset? StockDate { get; set; }

    }
}
