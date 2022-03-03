using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
   public class Terminal_Smf:EntityBase
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Terminal { get; set; }
        public Guid StoreId { get; set; }
        public string Employee_Number { get; set; }
        public DateTimeOffset? Inventory_Date { get; set; }
        public string Department { get; set; }
        public string Code { get; set; }
        public decimal Sale_Price { get; set; }
        public int tag { get; set; }
        public int shelf { get; set; }
        public OperationType operation { get; set; }
        public DateTime creation_time { get; set; }
        public string inventory_key { get; set; }
        public bool sync_to_terminal_status { get; set; }
        public DateTime sync_to_terminal_time { get; set; }
        public bool sync_back_from_terminal_status { get; set; }
        public DateTime sync_back_from_terminal_time { get; set; }
        public int count_type { get; set; }
        public int total_counted { get; set; }
        public DateTime count_time { get; set; }
        public bool nof { get; set; }
        public bool counted_status { get; set; }
       // public virtual OrderJob OrderJob { get; set; }
        public virtual Store Store { get; set; }
        public virtual Customer customer { get; set; }



        public virtual Terminal_Department MF2 { get; set; }
    }
}
