using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TOMI.Data.Database.Entities;
using TOMI.Services.Models;

namespace TOMI.Services.Interfaces
{
    public interface IStoreService
    {
        Task<StoreModelResponse> CreateStore(StoreModel user);
        Task<GetStoreListResponse> GetUserByCustomereAsync(string customerId);
        Task<FileUplaodRespone> SalesData(SalesDataModel stockModel);
        Task<FileUplaodRespone> MasterData(MasterDataModel masterData);
        Task<FileUplaodRespone> StocksData(StocksDataModel stocksData);
        Task<List<Sales>> GetSalesData(SalesModelRequest request);
        Task<List<Master>> GetMasterData(MasterModelRequest request);
        Task<List<Stocks>> GetStocksData(StocksModelRequest request);






    }
}
