using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
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
                   await  _context.SaveChangesAsync();
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
        public async Task<FileUplaodRespone> SalesData(SalesDataModel stockModel)
        {
            bool isSaveSuccess = false;
            string fileName;
            List<SalesDetailResponse> records = new List<SalesDetailResponse>();
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
              
                using (var reader = new StreamReader(path))
                using (var csv = new CsvReader(reader, config))
                {
                    records = csv.GetRecords<SalesDetailResponse>().ToList();

                    var storedetails = _mapper.Map<List<Sales>>(records);
                    //Loop and insert records.  
                    foreach (Sales storedetail in storedetails)
                    {
                        storedetail.CustomerId = stockModel.CustomerId;
                        storedetail.StoreId = stockModel.StoreId;
                        storedetail.StockDate = stockModel.StockDate;


                        _context.Sales.Add(storedetail);
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
                   await _context.SaveChangesAsync();
                }
                isSaveSuccess = true;
            }
            catch (Exception e)
            {
                //log error
            }

             return new FileUplaodRespone
            {
                 stockRecordCount = records.Count.ToString(),
                Success = isSaveSuccess
             }; ;
        }
        public async Task<FileUplaodRespone> MasterData(MasterDataModel masterData)
        {
            bool isSaveSuccess = false;
            string fileName;
            List<MasterDataResponse> records = new List<MasterDataResponse>();
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
                var temp = File.ReadAllLines(path);
               

                string regex = "^0+(?!$)";
                foreach (string line in temp)
                {
                    MasterDataResponse masterdata = new MasterDataResponse();
                    masterdata.SKU = Regex.Replace(line.Substring(0, 26), regex, "");
                    masterdata.Barcode = (line.Substring(27, 30).Trim());
                    masterdata.RetailPrice = Regex.Replace(line.Substring(58, 11), regex, ""); 
                    masterdata.Description = (line.Substring(70, 40).Trim());
                    masterdata.Department = (line.Substring(110, 02).Trim());
                    masterdata.Blank = (line.Substring(112,11).Trim());
                    masterdata.OHQuantity = "0";
                    masterdata.Unity = (line.Substring(134, 3).Trim());
                    records.Add(masterdata);
                }
            
                    var storedetails = _mapper.Map<List<Master>>(records);
                    //Loop and insert records.  
                    foreach (Master storedetail in storedetails)
                    {
                        storedetail.CustomerId = masterData.CustomerId;
                        storedetail.StoreId = masterData.StoreId;
                        storedetail.StockDate = masterData.StockDate;


                        _context.Master.Add(storedetail);
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

             return new FileUplaodRespone
            {
                stockRecordCount = records.Count.ToString(),
                Success = isSaveSuccess
            }; ; ;
        }

        public async Task<FileUplaodRespone> StocksData(StocksDataModel stocksData)
        {
            bool isSaveSuccess = false;
            string fileName;
            List<StocksDataResponse> records = new List<StocksDataResponse>();
            try
            {
                var extension = "." + stocksData.File.FileName.Split('.')[stocksData.File.FileName.Split('.').Length - 1];
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
                    await stocksData.File.CopyToAsync(stream);
                }
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false,
                    BadDataFound = null,
                    Delimiter = "|",
                };
                var temp = File.ReadAllLines(path);
                string regex = "^0+(?!$)";
                foreach (string line in temp)
                {
                    StocksDataResponse stockdata = new StocksDataResponse();
                    stockdata.SKU = Regex.Replace(line.Substring(0, 26), regex, "");
                    stockdata.Barcode = (line.Substring(27, 30).Trim());
                    stockdata.RetailPrice = Regex.Replace(line.Substring(58, 11), regex, "");
                    stockdata.Description = (line.Substring(70, 40).Trim());
                    stockdata.Department = (line.Substring(110, 02).Trim());
                    stockdata.Blank = (line.Substring(112, 11).Trim());
                    stockdata.OHQuantity = (line.Substring(122, 11).Trim());
                    stockdata.Unity = (line.Substring(134, 3).Trim());

                    records.Add(stockdata);
                }
               
                var storedetails = _mapper.Map<List<Stocks>>(records);
               //Loop and insert records.  
               foreach (Stocks storedetail in storedetails)
                {
                    storedetail.CustomerId = stocksData.CustomerId;
                  storedetail.StoreId = stocksData.StoreId;
                  storedetail.StockDate = stocksData.StockDate;


                 _context.Stocks.Add(storedetail);
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
                  
                    _context.SaveChanges();
                }
                isSaveSuccess = true;
            }
            catch (Exception e)
            {
                //log error
            }

            return new FileUplaodRespone
            {
                stockRecordCount = records.Count.ToString(),
                Success = isSaveSuccess
            }; ; ;
        }



        public async Task<List<Sales>> GetSalesData(SalesModelRequest request)
        {
            var response = await _context.Sales.Where(c => c.CustomerId == request.CustomerId && c.StoreId == request.StoreId && c.StockDate == request.StockDate).ToListAsync();
            return response;
        }
        public async Task<List<Master>> GetMasterData(MasterModelRequest request)
        {
            var response = await _context.Master.Where(c => c.CustomerId == request.CustomerId && c.StoreId == request.StoreId && c.StockDate == request.StockDate).ToListAsync();
            return response;
        }

        public async Task<List<Stocks>> GetStocksData(StocksModelRequest request)
        {
            var response = await _context.Stocks.Where(c => c.CustomerId == request.CustomerId && c.StoreId == request.StoreId && c.StockDate == request.StockDate).ToListAsync();
            return response;
        }
    }
}
