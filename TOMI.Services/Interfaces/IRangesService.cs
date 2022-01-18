using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;
using TOMI.Services.Models;

namespace TOMI.Services.Interfaces.RangesService
{
    public interface IRangesService
    {
        Task<Ranges> SaveRanges(RangesModel customer);
        Task<Ranges> DeleteRange(Guid id);
        Task<Ranges> GetRange(Guid id);
        Task<List<Ranges>> GetRangesAsync();
        Task<List<Ranges>> Search(string name);
    }
}
