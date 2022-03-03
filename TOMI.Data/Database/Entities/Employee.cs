using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
   public class Employee
    {
        [Key]
        public string EmpNumber { get; set; }
        public string EmpName { get; set; }
        public string LastName { get; set; }
        public string Postion { get; set; }
        public string inventory_key { get; set; }
    }
}
