using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;

namespace TOMI.Services.Models
{
   
        public class OrderjobResponse : BaseResponse
        {
            public OrderJob orderJob { get; set; }
        }

    public class OrderResponse : BaseResponse
    {
        public List<OrderJob> orderJob { get; set; }
    }







}
