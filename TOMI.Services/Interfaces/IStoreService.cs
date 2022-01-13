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
        Task<FileUplaodRespone> StocksData(StockModel stockModel);
        Task<FileUplaodRespone> MasterData(MasterDataModel masterData);
        Task<List<Stock>> GetStockData(StockModelRequest request);
        Task<List<Master>> GetMasterData(MasterModelRequest request);


    }
}
