using System;
using Microsoft.AspNetCore.Http;

namespace TOMI.Services.Models
{
    public class FilterDataRequest
    {
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTime? StockDate { get; set; }
    }

    public class FilterDataModel
    {
        public IFormFile File { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTime? StockDate { get; set; }
    }
    public class TerminalModel
    {
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTime? Date { get; set; }
        public int CountType { get; set; }
        public string InventaryKey { get; set; }
    }

}
