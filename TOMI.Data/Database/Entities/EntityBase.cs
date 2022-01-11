using System;

namespace TOMI.Data.Database.Entities
{
    public class EntityBase
    {
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public Guid? Deletedby { get; set; }
        public bool IsActive { get; set; }  

    }
}
