using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;

namespace TOMI.Services.Models
{
   public class GroupModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
    }

    public class GroupResponse : BaseResponse
    {
        public Group group { get; set; }
    }
}
