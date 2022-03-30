using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;

namespace TOMI.Services.Models
{
    public class MergeWithNewInfloarding
    {
        public int Tag { get; set; }
        public string EmpNumber { get; set; }
        public string Terminal { get; set; }
        public string EmpNumberOriginal { get; set; }
        public string TerminalOriginal { get; set; }
    }
    public class MergeWithNewResponse : BaseResponse
    {
        public Terminal_Smf terminal_smf { get; set; }
    }
}
