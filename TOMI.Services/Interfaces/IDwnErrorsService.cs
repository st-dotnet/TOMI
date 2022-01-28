using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;
using TOMI.Services.Models;

namespace TOMI.Services.Interfaces
{
    public interface IDwnErrorsService
    {
        Task<DwnErrorsResponse> SaveDwnErrors(DwnErrorsModel model);
        Task<DwnErrors> DeleteDwnErrors(int Id);
        Task<DwnErrors> GetDwnErrorsAsync(int id);
        Task<List<DwnErrors>> GetDwnErrorsListAsync();

    }
}
