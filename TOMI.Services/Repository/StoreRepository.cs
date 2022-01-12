﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TOMI.Data.Database;
using TOMI.Data.Database.Entities;
using TOMI.Services.Interfaces;
using TOMI.Services.Models;

namespace TOMI.Services.Repository
{
    public class StoreRepository : IStoreService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<StoreRepository> _logger;
        private readonly TOMIDataContext _context;
        public StoreRepository(ILogger<StoreRepository> logger, TOMIDataContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }
        public async Task<StoreModelResponse> CreateStore(StoreModel store)
        {
            try
            {
                Store exsitingCustomer = await _context.Stores.FirstOrDefaultAsync(c => c.Name == store.Name && c.CustomerId == store.CustomerId);
                if (exsitingCustomer == null)
                {
                    var customers = _mapper.Map<Store>(store);
                    Store result = _context.Stores.Add(customers).Entity;
                    _context.SaveChanges();
                    return new StoreModelResponse
                    {
                        store = result,
                        Success = true
                    };
                }
                else
                    return new StoreModelResponse { Error = " Store Already Exist" };
            }
            catch (Exception ex)
            {
                return new StoreModelResponse { Error = ex.Message };
            }

        }
        public async Task<GetStoreListResponse> GetUserByCustomereAsync(string customerId)
        {
            {
                var stores = await _context.Stores.Include(c => c.Customer).Where(x => x.CustomerId.ToString() == customerId).ToListAsync();
                return new GetStoreListResponse
                {
                    store = stores,
                    Success = true
                };

            }
        }
        public async Task<bool> StocksData(StockModel stockModel)
        {
            bool isSaveSuccess = false;
            string fileName;

            try
            {
                var extension = "." + stockModel.File.FileName.Split('.')[stockModel.File.FileName.Split('.').Length - 1];
                fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.

                var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");

                if (!Directory.Exists(pathBuilt))
                {
                    Directory.CreateDirectory(pathBuilt);
                }

                var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files",
                   fileName);


                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await stockModel.File.CopyToAsync(stream);
                }
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false,
                    BadDataFound = null,
                    Delimiter = "|",
                };
                List<StoreDetailsResponse> records = new List<StoreDetailsResponse>();
                using (var reader = new StreamReader(path))
                using (var csv = new CsvReader(reader, config))
                {
                    records = csv.GetRecords<StoreDetailsResponse>().ToList();

                    var storedetails = _mapper.Map<List<Stock>>(records);
                    //Loop and insert records.  
                    foreach (Stock storedetail in storedetails)
                    {
                        storedetail.CustomerId = stockModel.CustomerId;
                        storedetail.StoreId = stockModel.StoreId;
                        storedetail.StockDate = stockModel.StockDate;


                        _context.Stocks.Add(storedetail);
                    }
                }
                // Submit the change to the database.
                try
                {
                    await _context.SaveChangesAsync();
                    File.Delete(path);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Make some adjustments.
                    // ...
                    // Try again.
                    _context.SaveChanges();
                }
                isSaveSuccess = true;
            }
            catch (Exception e)
            {
                //log error
            }

            return isSaveSuccess;
        }
        public async Task<bool> MasterData(MasterDataModel masterData)
        {
            bool isSaveSuccess = false;
            string fileName;

            try
            {
                var extension = "." + masterData.File.FileName.Split('.')[masterData.File.FileName.Split('.').Length - 1];
                fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.

                var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");

                if (!Directory.Exists(pathBuilt))
                {
                    Directory.CreateDirectory(pathBuilt);
                }

                var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files",
                   fileName);


                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await masterData.File.CopyToAsync(stream);
                }
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false,
                    BadDataFound = null,
                    Delimiter = "|",
                };
                List<MasterDataResponse> records = new List<MasterDataResponse>();
                using (var reader = new StreamReader(path))
                using (var csv = new CsvReader(reader, config))
                {
                    records = csv.GetRecords<MasterDataResponse>().ToList();

                    var storedetails = _mapper.Map<List<Master>>(records);
                    //Loop and insert records.  
                    foreach (Master storedetail in storedetails)
                    {
                        storedetail.CustomerId = masterData.CustomerId;
                        storedetail.StoreId = masterData.StoreId;
                        storedetail.StockDate = masterData.StockDate;


                        _context.MasterData.Add(storedetail);
                    }
                }
                // Submit the change to the database.
                try
                {
                    await _context.SaveChangesAsync();
                    File.Delete(path);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Make some adjustments.
                    // ...
                    // Try again.
                    _context.SaveChanges();
                }
                isSaveSuccess = true;
            }
            catch (Exception e)
            {
                //log error
            }

            return isSaveSuccess;
        }
        public async Task<List<Stock>> GetStockData(StockModelRequest request)
        {
            var response = await _context.Stocks.Where(c => c.CustomerId == request.CustomerId && c.StoreId == request.StoreId && c.StockDate == request.StockDate).ToListAsync();
            return response;
        }
        public async Task<List<Master>> GetMasterData(MasterModelRequest request)
        {
            var response = await _context.MasterData.Where(c => c.CustomerId == request.CustomerId && c.StoreId == request.StoreId && c.StockDate == request.StockDate).ToListAsync();
            return response;
        }
    }
}
