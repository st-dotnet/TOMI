using System;
using System.Collections.Generic;
using TOMI.Data.Database.Entities;

namespace TOMI.Services.Models
{
    public class RangesModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public Guid? GroupId { get; set; }
        public string TagFrom { get; set; }
        public string TagTo { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTimeOffset? StockDate { get; set; }
    }
    public class RangesResponse : BaseResponse
    {
        public Ranges range { get; set; }
    }
    public class GetRangesListResponse : BaseResponse
    {
        public IList<GetCustomerResponse> Agents { get; set; }
    }
    public class GetRangesResponse : BaseResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string TagFrom { get; set; }
        public string TagTo { get; set; }
    }
}
