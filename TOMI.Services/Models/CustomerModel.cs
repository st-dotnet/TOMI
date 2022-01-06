using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;

namespace TOMI.Services.Models
{
    public class CustomerModel
    {
        public string Name { get; set; }
    }
    public class CustomerModelResponse : BaseResponse
    {
        public Customer customer { get; set; }
    }
    public class GetCustomerListResponse : BaseResponse
    {
        public IList<GetCustomerResponse> Agents { get; set; }
    }
    public class GetCustomerResponse : BaseResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string AsstEmail { get; set; }
        public string AsstPhone { get; set; }
        public string Note { get; set; }
        public long? ProviderId { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public string UpdatedName { get; set; }
        public string CreatedName { get; set; }
    }
}
