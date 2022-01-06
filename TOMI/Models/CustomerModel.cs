using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;

namespace TOMI.Web.Models
{
    public class CustomerModel: BaseResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    public class CustomerModelResponse : BaseResponse
    {
        public Customer customer { get; set; }
    }
}
