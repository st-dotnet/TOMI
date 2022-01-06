using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;

namespace TOMI.Services.Interfaces
{
   public interface IUserService
    {
        User Authenticate(string username, string password);

    }
}
