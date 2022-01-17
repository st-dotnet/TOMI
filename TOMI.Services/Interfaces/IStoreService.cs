using System.Collections.Generic;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;
using TOMI.Services.Models;

namespace TOMI.Services.Interfaces
{
    public interface IStoreService
    {
        Task<StoreModelResponse> CreateStore(StoreModel user);
        Task<GetStoreListResponse> GetUserByCustomereAsync(string customerId);
        Task<FileUplaodRespone> SalesData(FilterDataModel stockModel);
        Task<FileUplaodRespone> MasterData(FilterDataModel masterData);
        Task<FileUplaodRespone> StocksData(FilterDataModel stocksData);
        Task<List<Sales>> GetSalesData(FilterDataRequest request);
        Task<List<Master>> GetMasterData(FilterDataRequest request);
        Task<List<Stocks>> GetStocksData(FilterDataRequest request);
    }
}
