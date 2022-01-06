using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TOMI.Services.Models
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public string Information { get; set; }
        public DateTimeOffset? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
    }
}
