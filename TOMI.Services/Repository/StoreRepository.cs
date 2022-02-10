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
                throw new Exception(ex.ToString());
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
                string existingFile = model.File.FileName.ToString();
                if (existingFile.Contains("DPTO"))
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
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());

                    }
                    isSaveSuccess = true;
                }
                else
                {
                    return new FileUplaodRespone { Success = false, Error = "Invalid File" };
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
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
                string existingFile = model.File.FileName.ToString();
                if (existingFile.Contains("EXIS"))
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
                    string regex = "^0+(?!$)";


                    stockList = stockFile.Skip(1).SelectMany(x => new List<Stock>
                {
                   new()
                    {
                        Store=Regex.Replace(x.Substring(0,4), regex, ""),
                        SKU = Regex.Replace(x.Substring(4,14), regex, ""),
                        Departament = Regex.Replace(x.Substring(18,4), regex, ""),
                        Description = (x.Substring(22,30).Trim()),
                        PrecVtaNorm= Regex.Replace(x.Substring(52,8), regex, ""),
                        PrecVtaNorm_SImpto= Regex.Replace(x.Substring(60,8), regex, ""),
                        SOH= Regex.Replace(x.Substring(68,12), regex, ""),
                        Category=x.Length ==87 ?x.Substring(81,6):null,
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
                else
                {
                    return new FileUplaodRespone { Success = false, Error = "Invalid File" };
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
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
                string existingFile = model.File.FileName.ToString();
                if (existingFile.Contains("APAR"))
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
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }
                    isSaveSuccess = true;
                }
                else
                {
                    return new FileUplaodRespone { Success = false, Error = "Invalid File" };
                }

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
                string existingFile = model.File.FileName.ToString();
                if (existingFile.Contains("CATE"))
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
                else
                {
                    return new FileUplaodRespone { Success = false, Error = "Invalid File" };
                }
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
                                CustomerId = model.CustomerId,
                                StoreId = model.StoreId,
                                StockDate = model.StockDate
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
        public async Task<List<Departments>> GetDepartmentsData(FilterDataRequest request)
        {
            try
            {
                return await _context.Departments.Where(c => c.CustomerId == request.CustomerId && c.StoreId == request.StoreId && c.StockDate.Value.Date == request.StockDate.Value.Date).Take(500).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<List<Reserved>> GetReservedData(FilterDataRequest request)
        {
            try
            {
                return await _context.Reserved.Where(c => c.CustomerId == request.CustomerId && c.StoreId == request.StoreId && c.StockDate.Value.Date == request.StockDate.Value.Date).Take(500).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<List<Stock>> GetNewStockData(FilterDataRequest request)
        {
            try
            {
                return await _context.Stock.Where(c => c.CustomerId == request.CustomerId && c.StoreId == request.StoreId && c.StockDate.Value.Date == request.StockDate.Value.Date).Take(500).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<List<Categories>> GetCategoriesData(FilterDataRequest request)
        {
            try
            {
                return await _context.Categories.Where(c => c.CustomerId == request.CustomerId && c.StoreId == request.StoreId && c.StockDate.Value.Date == request.StockDate.Value.Date).Take(500).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }
        public async Task<List<ParametersByDepartment>> GetParametersByDepartmentData(FilterDataRequest request)
        {
            try
            {
                return await _context.ParametersByDepartment.Where(c => c.CustomerId == request.CustomerId && c.StoreId == request.StoreId && c.StockDate.Value.Date == request.StockDate.Value.Date).Take(500).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }
        public async Task<List<OrderJob>> GetOrderJob(FilterDataRequest request)
        {
            try
            {
                return await _context.OrderJob.Where(c => c.CustomerId == request.CustomerId && c.StoreId == request.StoreId && c.StockDate.Value.Date == request.StockDate.Value.Date).Take(500).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
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
                string existingFile = model.File.FileName.ToString();
                if (existingFile.Contains("MAST"))
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
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }
                    isSaveSuccess = true;
                }

                else
                {
                    return new FileUplaodRespone { Success = false, Error = "Invalid File" };
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
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
