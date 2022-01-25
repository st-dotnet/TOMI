using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;
using TOMI.Services.Models;

namespace TOMI.Services.Interfaces.RangesService
{
    public interface IRangesService
    {
        Task<RangesModel> SaveRanges(RangesModel customer);
       Task<Ranges> DeleteRange(Guid id);
        Task<Ranges> GetRange(Guid id);
        Task<List<Ranges>> GetRangesAsync();
        Task<Ranges> GetLastRange(Guid id);
        Task<int> GetMinMaxRange();



    }
}
