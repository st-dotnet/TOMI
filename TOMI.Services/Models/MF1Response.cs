using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Services.Models
{
    public class MF1Response
    {
        public List<TerminalSmf> TerminalSmf { get; set; }
    }
    public class TerminalSmf
    {
        public Guid Customer { get; set; }
        public string Terminal { get; set; }
        public string Store { get; set; }
        public string EmployeeNumber { get; set; }
        public DateTimeOffset InventoryDate { get; set; }
        public string Department { get; set; }
        public string Code { get; set; }
        public decimal SalePrice { get; set; }
        public string Tag { get; set; }
        public string Shelf { get; set; }
        public int Operation { get; set; }
        public string InventoryKey { get; set; }
        public int CountType { get; set; }
        public int TotalCounted { get; set; }
        public DateTime CountTime { get; set; }

        public string Description { get; set; }
        public bool Nof { get; set; }
        public bool CountedStatus { get; set; }


    }

    public class Empdata
    {
        public string EmpNumber { get; set; }
        public string EmpName { get; set; }
        public string LastName { get; set; }
        //public string Postion { get; set; }
        //public string inventory_key { get; set; }

    }

    //public class Ids
    //{
    //    public Guid CustomerId { get; set; }
    //    public Guid StoreId { get; set; }

    //    public string Description { get; set; }


    //}

    public class EmpdataDataResponse
    {
        public List<Empdata> Empdata { get; set; }
    }

    public class MF1AndEmp
    {
        public List<TerminalSmf> TerminalSmf { get; set; }
        public List<Empdata> Employees { get; set; }
        public Guid CustomerId { get; set; }
        public Guid StoreId { get; set; }

    }


}
