using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
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
        private IConfiguration _configuration;
        private IWebHostEnvironment _environment;
        public StoreRepository(ILogger<StoreRepository> logger, TOMIDataContext context, IMapper mapper, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _environment = environment;
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
                    await _context.SaveChangesAsync();
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
        public async Task<FileUplaodRespone> SalesData(FilterDataModel stockModel)
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
        public async Task<FileUplaodRespone> MasterData(FilterDataModel masterData)
        {
            bool isSaveSuccess = false;
            string fileName;

            List<Master> masterList = new();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            double timeElapsed = 0;

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
                var masterFile = File.ReadAllLines(path);

                string regex = "^0+(?!$)";

                masterList = masterFile.SelectMany(x => new List<Master>
                {
                   new()
                    {
                        SKU = Regex.Replace(x.Substring(0, 26), regex, ""),
                        Barcode = (x.Substring(27, 30).Trim()),
                        RetailPrice = (Regex.Replace(x.Substring(58, 11), regex, "")).Replace(",", "."),
                        Description = (x.Substring(70, 40).Trim()),
                        Department = (x.Substring(110, 02).Trim()),
                        Blank = (x.Substring(112, 11).Trim()),
                        OHQuantity = "0",
                        Unity = (x.Substring(134, 3).Trim()),
                        CustomerId = masterData.CustomerId,
                        StoreId = masterData.StoreId,
                        StockDate = masterData.StockDate,
                    }
                 }).ToList();

                // Submit the change to the database.
                try
                {
                    await _context.BulkInsertAsync(masterList);
                    stopwatch.Stop();
                    timeElapsed = Math.Ceiling(stopwatch.Elapsed.TotalSeconds);

                    File.Delete(path);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                }
                isSaveSuccess = true;
            }
            catch (Exception e)
            {
                //log error
            }

            return new FileUplaodRespone
            {
                stockRecordCount = masterList.Count.ToString(),
                TimeElapsed = timeElapsed,
                Success = isSaveSuccess
            }; ; ;
        }

        public async Task<FileUplaodRespone> DepartmentsData(FilterDataModel model)
        {
            bool isSaveSuccess = false;
            string fileName;

            List<Departments> departmentsList = new();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            double timeElapsed = 0;

            try
            {
                var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
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
                    await model.File.CopyToAsync(stream);
                }
                var departmentFile = File.ReadAllLines(path);
                string regex = "^0+(?!$)";
                departmentsList = departmentFile.Skip(1).SelectMany(x => new List<Departments>
                {
                   new()
                    {
                       Division=Regex.Replace(x.Substring(0,2),regex, ""),
                       DivisionName=Regex.Replace(x.Substring(2,30),regex, ""),
                       Department=Regex.Replace(x.Substring(32,4),regex, ""),
                       DepartmentName=Regex.Replace(x.Substring(36,30),regex, ""),
                        CustomerId = model.CustomerId,
                        StoreId = model.StoreId,
                        StockDate = model.StockDate,
                    }
                 }).ToList();

                // Submit the change to the database.
                try
                {
                    await _context.BulkInsertAsync(departmentsList);
                    stopwatch.Stop();
                    timeElapsed = Math.Ceiling(stopwatch.Elapsed.TotalSeconds);

                    File.Delete(path);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                }
                isSaveSuccess = true;
            }
            catch (Exception e)
            {
                //log error
            }

            return new FileUplaodRespone
            {
                stockRecordCount = departmentsList.Count.ToString(),
                TimeElapsed = timeElapsed,
                Success = isSaveSuccess
            }; ; ;
        }
        public async Task<FileUplaodRespone> StockData(FilterDataModel model)
        {
            bool isSaveSuccess = false;
            string fileName;

            List<Stock> stockList = new();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            double timeElapsed = 0;

            try
            {
                var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
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
                    await model.File.CopyToAsync(stream);
                }
                var stockFile = File.ReadAllLines(path);



                stockList = stockFile.Skip(1).SelectMany(x => new List<Stock>
                {
                   new()
                    {
                        Store=(x.Substring(0,4)),
                        SKU=(x.Substring(4,14)),
                        Departament=(x.Substring(18,4)),
                        Description=(x.Substring(22,30)),
                        PrecVtaNorm=(x.Substring(52,8)),
                        PrecVtaNorm_SImpto=(x.Substring(60,8)),
                        SOH=(x.Substring(68,12)),
                        Category=x.Length  ==87 ?x.Substring(81,6):null,
                        CustomerId = model.CustomerId,
                        StoreId = model.StoreId,
                        StockDate = model.StockDate,
                    }
                 }).ToList();

                // Submit the change to the database.
                try
                {
                    await _context.BulkInsertAsync(stockList);
                    stopwatch.Stop();
                    timeElapsed = Math.Ceiling(stopwatch.Elapsed.TotalSeconds);

                    File.Delete(path);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                }
                isSaveSuccess = true;
            }
            catch (Exception e)
            {
                //log error
            }

            return new FileUplaodRespone
            {
                stockRecordCount = stockList.Count.ToString(),
                TimeElapsed = timeElapsed,
                Success = isSaveSuccess
            }; ; ;
        }
        public async Task<FileUplaodRespone> ReservedData(FilterDataModel model)
        {
            bool isSaveSuccess = false;
            string fileName;

            List<Reserved> reservedList = new();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            double timeElapsed = 0;

            try
            {
                var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.

               
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files",
                 fileName);

                string regex = "^0+(?!$)";
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }
                var reservedFile = File.ReadAllLines(path);

                reservedList = reservedFile.Skip(1).SelectMany(x => new List<Reserved>
                {
                   new()
                    {
                        Store=Regex.Replace(x.Substring(0,4),regex,""),
                        Code=Regex.Replace(x.Substring(4,14),regex,""),
                        Quantity=Regex.Replace(x.Substring(18,9),regex,""),
                        Filler=Regex.Replace(x.Substring(27,1),regex,""),
                        CustomerId = model.CustomerId,
                        StoreId = model.StoreId,
                        StockDate = model.StockDate,
                    }
                 }).ToList();

                // Submit the change to the database.
                try
                {
                    await _context.BulkInsertAsync(reservedList);
                    stopwatch.Stop();
                    timeElapsed = Math.Ceiling(stopwatch.Elapsed.TotalSeconds);

                    File.Delete(path);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                }
                isSaveSuccess = true;
            }
            catch (Exception e)
            {
                //log error
            }

            return new FileUplaodRespone
            {
                stockRecordCount = reservedList.Count.ToString(),
                TimeElapsed = timeElapsed,
                Success = isSaveSuccess
            }; ; ;
        }

        public async Task<FileUplaodRespone> CatergoriesData(FilterDataModel model)
        {
            bool isSaveSuccess = false;
            string fileName;

            List<Categories> catergoriesList = new();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            double timeElapsed = 0;

            try
            {
                var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
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
                    await model.File.CopyToAsync(stream);
                }
                var catergoriesFile = File.ReadAllLines(path);
                string regex = "^0+(?!$)";
                catergoriesList = catergoriesFile.Skip(1).SelectMany(x => new List<Categories>
                {
                   new()
                    {
                        Division=Regex.Replace(x.Substring(0,2),regex,""),
                        DivisionName=Regex.Replace(x.Substring(2,30),regex,""),
                        Category=Regex.Replace(x.Substring(32,6),regex,""),
                        CategoryName=Regex.Replace(x.Substring(38,40),regex,""),
                        CustomerId = model.CustomerId,
                        StoreId = model.StoreId,
                        StockDate = model.StockDate,
                    }
                 }).ToList();

                // Submit the change to the database.
                try
                {
                    await _context.BulkInsertAsync(catergoriesList);
                    stopwatch.Stop();
                    timeElapsed = Math.Ceiling(stopwatch.Elapsed.TotalSeconds);

                    File.Delete(path);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                }
                isSaveSuccess = true;
            }
            catch (Exception e)
            {
                //log error
            }

            return new FileUplaodRespone
            {
                stockRecordCount = catergoriesList.Count.ToString(),
                TimeElapsed = timeElapsed,
                Success = isSaveSuccess
            }; ; ;
        }

        public async Task<FileUplaodRespone> ParametersByDepartmentData(FilterDataModel model)
        {
            bool isSaveSuccess = false;
            List<ParametersByDepartment> parametersbydepartmentList = new();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            double timeElapsed = 0;
            try
            {
                using (var stream = new MemoryStream())
                {
                    await model.File.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                        var rowcount = worksheet.Dimension.Rows;
                        var colcount = worksheet.Dimension.Columns;
                        for (int row = 2; row < rowcount; row++)
                        {
                            parametersbydepartmentList.Add(new ParametersByDepartment
                            {
                                Department = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                Quantity = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                Pesos = worksheet.Cells[row, 3].Value.ToString().Trim(),
                                PercentageInPieces = worksheet.Cells[row, 4].Value.ToString().Trim(),
                                CustomerId=model.CustomerId,
                                StoreId=model.StoreId,
                                StockDate=model.StockDate
                            });
                        }
                    }
                }
                //SaveDataToDb(list);
                _context.ParametersByDepartment.AddRange(parametersbydepartmentList);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                //log error
            }
            isSaveSuccess = true;
            return new FileUplaodRespone
            {
                stockRecordCount = parametersbydepartmentList.Count.ToString(),
                TimeElapsed = timeElapsed,
                Success = isSaveSuccess
            };
        }
        public async Task<FileUplaodRespone> StocksData(FilterDataModel stocksData)
        {
            bool isSaveSuccess = false;
            string fileName;
            List<Stocks> stockList = new();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            double timeElapsed = 0;

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

                var stockfile = File.ReadAllLines(path);
                string regex = "^0+(?!$)";

                stockList = stockfile.SelectMany(x => new List<Stocks>
                {
                   new()
                    {
                        SKU = Regex.Replace(x.Substring(0, 26), regex, ""),
                        Barcode = (x.Substring(27, 30).Trim()),
                        RetailPrice = (Regex.Replace(x.Substring(58, 11), regex, "")).Replace(",", "."),
                        Description = (x.Substring(70, 40).Trim()),
                        Department = (x.Substring(110, 02).Trim()),
                        Blank = (x.Substring(112, 11).Trim()),
                        OHQuantity = Regex.Replace(x.Substring(122, 11), regex, "").Trim(),
                        Unity = (x.Substring(134, 3).Trim()),
                        CustomerId = stocksData.CustomerId,
                        StoreId = stocksData.StoreId,
                        StockDate = stocksData.StockDate,
                    }
                 }).ToList();


                //var isMasterSkuExist = _context.Master.FirstOrDefault(x => x.SKU == stockList[0].SKU);
                //if (isMasterSkuExist != null)
                //{
                //    isMasterSkuExist.OHQuantity = stockList[0].OHQuantity;
                //    _context.Master.Update(isMasterSkuExist);

                //}


                //var storedetails = _mapper.Map<List<Stocks>>(records);
                ////Loop and insert records.  
                //foreach (Stocks storedetail in storedetails)
                //{

                //    storedetail.CustomerId = stocksData.CustomerId;
                //    storedetail.StoreId = stocksData.StoreId;
                //    storedetail.StockDate = stocksData.StockDate;

                //    _context.Stocks.Add(storedetail);

                //}

                try
                {
                    await _context.BulkInsertAsync(stockList);
                    stopwatch.Stop();
                    timeElapsed = Math.Ceiling(stopwatch.Elapsed.TotalSeconds);

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

            }

            return new FileUplaodRespone
            {
                stockRecordCount = stockList.Count.ToString(),
                TimeElapsed = timeElapsed,
                Success = isSaveSuccess
            }; ; ;
        }



        public async Task<List<Sales>> GetSalesData(FilterDataRequest request)
        {
            return await _context.Sales.Where(c => c.CustomerId == request.CustomerId && c.StoreId == request.StoreId && c.StockDate.Value.Date == request.StockDate.Value.Date).Take(500).ToListAsync();

        }
        public async Task<List<Master>> GetMasterData(FilterDataRequest request)
        {
            return await _context.Master.Where(c => c.CustomerId == request.CustomerId && c.StoreId == request.StoreId && c.StockDate.Value.Date == request.StockDate.Value.Date).Take(500).ToListAsync();

        }

        public async Task<List<Stocks>> GetStocksData(FilterDataRequest request)
        {
            return await _context.Stocks.Where(c => c.CustomerId == request.CustomerId && c.StoreId == request.StoreId && c.StockDate.Value.Date == request.StockDate.Value.Date).Take(500).ToListAsync();
        }

        public async Task<List<Departments>> GetDepartmentsData(FilterDataRequest request)
        {
            return await _context.Departments.Where(c => c.CustomerId == request.CustomerId && c.StoreId == request.StoreId && c.StockDate.Value.Date == request.StockDate.Value.Date).Take(500).ToListAsync();

        }

        public async Task<List<Reserved>> GetReservedData(FilterDataRequest request)
        {
            return await _context.Reserved.Where(c => c.CustomerId == request.CustomerId && c.StoreId == request.StoreId && c.StockDate.Value.Date == request.StockDate.Value.Date).Take(500).ToListAsync();

        }

        public async Task<List<Stock>> GetNewStockData(FilterDataRequest request)
        {
            return await _context.Stock.Where(c => c.CustomerId == request.CustomerId && c.StoreId == request.StoreId && c.StockDate.Value.Date == request.StockDate.Value.Date).Take(500).ToListAsync();

        }

        public async Task<List<Categories>> GetCategoriesData(FilterDataRequest request)
        {
            return await _context.Categories.Where(c => c.CustomerId == request.CustomerId && c.StoreId == request.StoreId && c.StockDate.Value.Date == request.StockDate.Value.Date).Take(500).ToListAsync();

        }
        public async Task<List<ParametersByDepartment>> GetParametersByDepartmentData(FilterDataRequest request)
        {
            return await _context.ParametersByDepartment.Where(c => c.CustomerId == request.CustomerId && c.StoreId == request.StoreId && c.StockDate.Value.Date == request.StockDate.Value.Date).Take(500).ToListAsync();

        }
        public async Task<List<OrderJob>> GetOrderJob(FilterDataRequest request)
        {
            return await _context.OrderJob.Where(c => c.CustomerId == request.CustomerId && c.StoreId == request.StoreId && c.StockDate.Value.Date == request.StockDate.Value.Date).Take(500).ToListAsync();

        }


        public async Task<FileUplaodRespone> OrderJobData(FilterDataModel model)
        {
            bool isSaveSuccess = false;
            string fileName;

            List<OrderJob> orderJobList = new();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            double timeElapsed = 0;

            try
            {
                var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
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
                    await model.File.CopyToAsync(stream);
                }
                var orderJobFile = File.ReadAllLines(path);

                string regex = "^0+(?!$)";
                orderJobList = orderJobFile.Skip(1).SelectMany(x => new List<OrderJob>
                {
                   new()
                    {

                        Store = Regex.Replace(x.Substring(0,3),regex, ""),
                        Code= Regex.Replace(x.Substring(3,14),regex, ""),
                        Department=Regex.Replace(x.Substring(18,4),regex, ""),
                        Description =Regex.Replace(x.Substring(22,30),regex, ""),
                        SalePrice= Regex.Replace(x.Substring(52,8),regex, ""),
                        PriceWithoutTaxes= Regex.Replace(x.Substring(60,8),regex, ""),
                        SKU = Regex.Replace(x.Substring(68, 12), regex, ""),
                        Category=x.Length  ==88 ?x.Substring(82,6):null,
                        CustomerId = model.CustomerId,
                        StoreId = model.StoreId,
                        StockDate = model.StockDate,

                    }
                 }).ToList();

                // Submit the change to the database.
                try
                {
                    await _context.BulkInsertAsync(orderJobList);
                    stopwatch.Stop();
                    timeElapsed = Math.Ceiling(stopwatch.Elapsed.TotalSeconds);

                    File.Delete(path);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                }
                isSaveSuccess = true;
            }
            catch (Exception e)
            {
                //log error
            }

            return new FileUplaodRespone
            {
                stockRecordCount = orderJobList.Count.ToString(),
                TimeElapsed = timeElapsed,
                Success = isSaveSuccess
            }; ; ;
        }

    }
}
