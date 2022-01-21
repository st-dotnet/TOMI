using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;
using TOMI.Services.Models;

namespace TOMI.Services.Interfaces
{
    public interface IStockAdjustmentService
    {
        Task<StockAdjustmentModel> SaveStockAdjustment(StockAdjustmentModel model);
        Task<StockAdjustment> DeleteStockAdjustment(Guid id);
        Task<StockAdjustment> GetStockAdjustmentAsync(Guid id);
        Task<List<StockAdjustment>> GetStockAdjustmentListAsync();
        Task<List<StockAdjustment>> GoToRecord(Guid id);
        Task<List<StockAdjustment>> GetDeletedRecord(Guid recid);
        Task<List<StockAdjustment>> ChangeDeletedRecStatus(Guid recid);
    }
}
