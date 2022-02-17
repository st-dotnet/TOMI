using System;
using Microsoft.AspNetCore.Http;
using TOMI.Data.Database.Entities;

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
        public string StoreName { get; set; }
    }
    public class TerminalModel
    {
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTime? Date { get; set; }
        public int CountType { get; set; }
        public string InventaryKey { get; set; }

    }

    public class MF2Model
    {
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTime? Date { get; set; }
        public string Department { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }



    public class TerminalResponse : BaseResponse
    {
        public MF1 MF1 { get; set; }
    }

    public class TerminalMF2Response : BaseResponse
    {
        public MF2 MF2 { get; set; }
    }
}
