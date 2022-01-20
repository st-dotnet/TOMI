using System;
using System.Collections.Generic;
using TOMI.Data.Database.Entities;

namespace TOMI.Services.Models
{
    public class StoreModel
    {
        public Guid CustomerId { get; set; }

        public string Name { get; set; }
    }
    public class StoreModelResponse : BaseResponse
    {
        public Store store { get; set; }
    }
    public class GetStoreListResponse : BaseResponse
    {
        public IList<Store> store { get; set; }
    }

    public class FileUplaodRespone : BaseResponse
    {
        public string stockRecordCount { get; set; }
        public double TimeElapsed { get; set; }
    }
}
