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

        Task<List<StockAdjustment>> GetLabelDetailsAsyncTest();
        Task<List<StockAdjustment>> GetLabelDetailsAsync(int? tagFrom,int? tagTo);
        Task<List<StockAdjustment>> GetCodeNotFoundAsync();
        Task<List<StockAdjustment>> GetExtendedPricesAsync();
        List<stockandorder> GetUncountedItemsAsync();
        List<StockAndStockAdjust> GetVariationBySKUAsync();
        Task<List<StockAdjustment>> GetCorrectionsReportAsync();
        List<DeptAndStockAdjust> GetBreakDownReportAsync();
        Task<List<StockAdjustment>> GetDateTimeCheckReport();
        Task<List<Departments>> GetDepartments();
        Task<List<Ranges>> GetRangesforReport();

        Task<string> InventoryFigure(FilterCustomerReportDataModel model);
        Task<string> InventoryDetail(FilterCustomerReportDataModel model);

        //MARBETE
        Task<string> MarbeteDetail(FilterCustomerReportDataModel model);
        Task<List<BillingReport>> GetBillingReport();
        Task<List<FileStore>> GetFileStoreAsync(FileStoreModel model);
        Task<List<spgetVoidTagData>> GetVoidTagDataAsync();
        Task<List<FileStore>> GetFileStore(FileStoreOffset model);
    }
}