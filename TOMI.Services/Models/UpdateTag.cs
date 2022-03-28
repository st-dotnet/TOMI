using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;

namespace TOMI.Services.Models
{
    public class UpdateTag
    {

        public int Tag { get; set; }
        public string EmpNumber { get; set; }
        public string Terminal { get; set; }
        public int NewTag { get; set; }
    }
        public class Terminal_SmfResponse : BaseResponse
        {
        public Terminal_Smf range { get; set; }
        }
        public class MergeWithNewInfloarding
        {
        public int Tag { get; set; }
        public string newEmpNumber { get; set; }
        public string newTerminal { get; set; }
        public string orginalEmpNumber { get; set; }
        public string orginalTerminal { get; set; }
       }
        public class MergeWithNewResponse: BaseResponse
        {
        public Terminal_Smf range { get; set; }
        }
 
}
