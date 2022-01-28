using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;

namespace TOMI.Services.Models
{
   public class DwnErrorsModel
    {
        public int id { get; set; }
        public int Tag { get; set; }
        public string ErrorMessage { get; set; }
        public string Term { get; set; }
        public string EmpNo { get; set; }
        public int Lines { get; set; }
        public int Qty { get; set; }
        public double ExtPrice { get; set; }
    }

    public class DwnErrorsResponse:BaseResponse
    {
        public DwnErrors dwnErrors { get; set; }
    }
}
