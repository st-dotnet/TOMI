using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;

namespace TOMI.Services.Models
{

   public class InfoLoadModel
    {
        public InfoLoadModel()
        {
            DownloadedErrors = false;
        }
        public int Id { get; set; }
        public string Terminal { get; set; }
        public int Send { get; set; }
        public string Emp { get; set; }
        public DateTime Downloaded { get; set; }
        public int Lines { get; set; }
        public int Qty { get; set; }
        public double ExtPrice { get; set; }
        public bool DownloadedErrors { get; set; }
    }

    public class InfoLoadResponse : BaseResponse
    {
        public InfoLoad InfoLoad { get; set; }
    }
}
