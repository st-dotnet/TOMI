using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;
using TOMI.Services.Models;

namespace TOMI.Services.Interfaces
{
    public interface IReportOptionService
    {

       Task<List<StockAdjustment>> GetLabelDetailsAsync(int tag);
       Task<List<StockAdjustment>> GetCodeNotFoundAsync();
       Task<List<StockAdjustment>> GetExtendedPricesAsync();
        List<stockandorder> GetUncountedItemsAsync(string department);
        List<StockAndStockAdjust> GetVariationBySKUAsync(string department);
        Task<List<StockAdjustment>> GetCorrectionsReportAsync();
        List<DeptAndStockAdjust> GetBreakDownReportAsync();
        Task<List<StockAdjustment>> GetDateTimeCheckReport();
        Task<List<Departments>> GetDepartments();
        Task<List<Ranges>> GetRangesforReport();
    }
}
