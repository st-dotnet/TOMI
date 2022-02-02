using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

    public class FilterInfoDataModel
    {
        public IFormFile File { get; set; }
      
    }

    public class FileInfoDataResponse : BaseResponse
    {
        public string InfoDataRecordCount { get; set; }
        public double TimeElapsed { get; set; }
    }
}
