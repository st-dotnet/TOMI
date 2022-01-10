﻿using System.Collections.Generic;
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
        Task<bool> WriteFile(IFormFile file);
    }
}
