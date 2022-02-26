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
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string inventory_key { get; set;}
    }
}
