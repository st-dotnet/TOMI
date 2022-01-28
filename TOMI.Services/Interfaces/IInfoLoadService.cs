using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;
using TOMI.Services.Models;

namespace TOMI.Services.Interfaces
{
   public interface IInfoLoadService
    {
        Task<InfoLoadResponse> SaveInfoLoad(InfoLoadModel model);
        Task<InfoLoad> DeleteInfoLoad(int id);
        Task<InfoLoad> GetInfoLoadAsync(int id);
        Task<List<InfoLoad>> GetInfoLoadListAsync();
    }
}
