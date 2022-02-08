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
        Task<StockAdjustmentResponse> SaveStockAdjustment(StockAdjustmentModel model);
        Task<StockAdjustment> DeleteStockAdjustment(Guid id);
        Task<StockAdjustment> GetStockAdjustmentAsync(Guid id);
        Task<List<StockAdjustment>> GetStockAdjustmentListAsync();
        Task<List<StockAdjustment>> GoToRecord(int recid);
        Task<List<StockAdjustment>> GetDeletedRecord();
        Task<List<StockAdjustment>> ChangeDeletedRecStatus(Guid recid);
        Task<MasterResponse> MasterDataBySku(string sku);
        Task<OrderjobResponse> MasterDataByBarCode(string barcode);
        Task<List<StockAdjustment>> FilterStock(StockAdjustmentFilterModel model);
        Task<List<Master>> GetMasterDataByCustomerId(Guid id);
    }
}
