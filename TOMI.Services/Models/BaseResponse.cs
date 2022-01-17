using System;

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
