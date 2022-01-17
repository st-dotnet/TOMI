using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;
using TOMI.Services.Models;

namespace TOMI.Services.Interfaces
{
   public interface IGroupService
    {
        Task<Group> AddGroup(GroupModel model);
        Task<Group> DeleteGroup(Guid id);
        Task<Group> GetGroup(Guid id);
        Task<List<Group>> GetGroupAsync();
        Task<Group> UpdateGroup(GroupModel model);
    }
}
