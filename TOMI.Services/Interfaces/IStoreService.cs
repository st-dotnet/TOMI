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
        Task<FileUplaodRespone> DepartmentsData(FilterDataModel model);
        Task<FileUplaodRespone> StockData(FilterDataModel model);
        Task<FileUplaodRespone> ReservedData(FilterDataModel model);
        Task<List<Departments>> GetDepartmentsData(FilterDataRequest request);
        Task<List<Reserved>> GetReservedData(FilterDataRequest request);
        Task<List<Stock>> GetNewStockData(FilterDataRequest request);
        Task<FileUplaodRespone> CatergoriesData(FilterDataModel model);
        Task<List<Categories>> GetCategoriesData(FilterDataRequest request);
        Task<FileUplaodRespone> ParametersByDepartmentData(FilterDataModel model);
        Task<List<ParametersByDepartment>> GetParametersByDepartmentData(FilterDataRequest request);
        Task<FileUplaodRespone> OrderJobData(FilterDataModel model);
        Task<List<OrderJob>> GetOrderJob(FilterDataRequest request);
    }
}
