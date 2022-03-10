using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
    public class spInformationTransmissionDetails
    {
        [Key]
        public int Id { get; set; }
        public string Shelf { get; set; }
        public string SKU { get; set; }
        public int Qty   { get; set; }
        public string Description { get; set; }
        public string EmployeeNumber { get; set; }
        public string Term { get; set; }
        public int Dload { get; set; }
       
    }
}
