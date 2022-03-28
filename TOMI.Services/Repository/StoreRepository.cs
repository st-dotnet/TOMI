using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly TOMIDataContext _context;
        private IConfiguration _configuration;
        private IWebHostEnvironment _environment;
        private const string txtFileExtension = ".txt";
        private const string zipFileExtension = ".zip";
        private const string masterFileExtension = "MAST";
        private const string deptFileExtension = "DPTO";
        private const string reservedFileExtension = "APAR";
        private const string masterFile = "MASTER";
        private const string deptFile = "DPTO DIV";
        private const string reservedFile = "APARTADOS";
        private const string stockFileExtension = "EXIS";
        private const string stockFile = "EXISTENCIA";
        private const string categoryFileExtension = "CATE";
        private const string categoryFile = "CATE DIV";
        private readonly ILoggerManager _logger;
        public StoreRepository(ILoggerManager loggerManager, TOMIDataContext context, IMapper mapper, IConfiguration configuration, IWebHostEnvironment environment)
        {
           // logger = LogManager.GetCurrentClassLogger();
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _environment = environment;
            _logger = loggerManager;
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
        public async Task<FileUplaodRespone> ParametersByDepartmentData(FilterDataModel model)
        {
            if (model.File == null || model.File.Length == 0)
                return new FileUplaodRespone
                {
                    Success = false,
                    Error = "File Not Selected"
                };
            string fileExtension = Path.GetExtension(model.File.FileName);
            if (fileExtension != ".xls" && fileExtension != ".xlsx")
                return new FileUplaodRespone
                {
                    Success = false,
                    Error = "File Not Selected"
                };
            string tempfileName = Path.GetExtension(model.File.FileName);
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
                        var rowcount = worksheet.Dimension.Rows + 1;
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
            //TimeZoneInfo localZones = TimeZoneInfo.Local;
            //DateTime currentDate = model.StockDate.Value;
            //logger.Info($"Get jobOrderDate : {currentDate}");

            //DateTime utcTimes = DateTime.UtcNow;
            //TimeZoneInfo myZones = TimeZoneInfo.CreateCustomTimeZone("Central Standard Time", new TimeSpan(-5, 0, 0), "Central Standard Time", "Central Standard Time");
            //DateTime custDateTimes = TimeZoneInfo.ConvertTimeFromUtc(utcTimes, myZones);

            _logger.LogInfo("OrderJobData method started");
            bool isSaveSuccess = false;
            string fileName;
            var innerDataError = model.StockDate.Value.Date.AddDays(-1).Day.ToString("#00") + model.StockDate.Value.Date.Month.ToString("#00") +
                model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();

            _logger.LogInfo("innerDataError for check error");
            var forInnerStockDate = string.Empty;
            var forInventoryDate = string.Empty;

            List<OrderJob> orderJobList = new();
            //for file header
            List<FileStore> fileStores = new();
            List<FileStore> trailerRecords = new();
            //for file name
            List<UploadFileName> uploadFileNames = new();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");
            _logger.LogInfo("get pathBuilt");
            if (Directory.Exists(pathBuilt))
            {
                DirectoryInfo di = new DirectoryInfo(pathBuilt);
                FileInfo[] files = di.GetFiles();
                foreach (FileInfo file in files)
                {
                    _logger.LogInfo("Delete file");
                    file.Delete();
                }
            }

            double timeElapsed = 0;
            try
            {
                _logger.LogInfo("First try started");
                var jobDate = model.StockDate.ToString();
                var jobOrderStore = model.StoreId.ToString();
                var jobOrderStoreName = model.StoreName.ToString().Substring(0, 4);

                var jobOrderPreviousDate = model.StockDate.Value.Date.Month.ToString("#00")
                    + model.StockDate.Value.Date.AddDays(-2).Day.ToString("#00");

                var forInnerPreviousStockDate = model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString()
                         + model.StockDate.Value.Date.Month.ToString("#00")
                         + model.StockDate.Value.Date.AddDays(-2).Day.ToString("#00");

                var jobOrderInnerPreviousDate = model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString()
                        + model.StockDate.Value.Date.Month.ToString("#00")
                        + model.StockDate.Value.Date.AddDays(-2).Day.ToString("#00");

                var jobOrderDate = model.StockDate.Value.Date.Month.ToString("#00") +
                   model.StockDate.Value.Date.AddDays(-1).Day.ToString("#00");

                _logger.LogInfo($"Get jobOrderDate : {jobOrderDate}");
                try
                {
                    forInnerStockDate = model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString()
                         + model.StockDate.Value.Date.Month.ToString("#00")
                         + model.StockDate.Value.Date.AddDays(-1).Day.ToString("#00");

                    _logger.LogInfo($"Get jobOrforInnerStockDatederDate : {forInnerStockDate}");
                }
                catch (Exception ex)
                {
                    return new FileUplaodRespone
                    {
                        Success = false,
                        Error = "Inner StockDate error"
                    };
                }

                //file name validation 

                var pathFile = model.File.FileName;
                _logger.LogInfo($"Get pathFile : {pathFile}");
                var tempfileName = new DirectoryInfo(pathFile).Name;
                _logger.LogInfo($"Get tempfileName : {tempfileName}");
                //file Name
                var filetext = tempfileName.Substring(0, 4);
                //file month
                var exDate = tempfileName.Substring(4, 4);
                var storeNumber = tempfileName.Substring(9, 4);

                _logger.LogInfo($"Get filetext : {filetext} , exDate : {exDate} , storeNumber : {storeNumber} ");
                var checkFileExtension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                _logger.LogInfo($"checkFileExtension : {checkFileExtension}");
                if (checkFileExtension == txtFileExtension || checkFileExtension == zipFileExtension)
                {
                    // if (filetext != masterFileExtension || jobOrderStoreName != storeNumber && (exDate.Equals((jobOrderPreviousDate))||exDate.Equals((jobOrderDate))))
                    //if (jobOrderPreviousDate == exDate )
                    //{
                    if (filetext != masterFileExtension || jobOrderStoreName != storeNumber || exDate!= jobOrderDate)
                    {
                        return new FileUplaodRespone
                        {
                            Success = false,
                            //  Error = $"Incorrect file with tempfileName : {tempfileName} , filetext:{filetext} , masterFileExtension:{masterFileExtension} , jobOrderStoreName:{jobOrderStoreName} , storeNumber:{storeNumber} , jobOrderDate:{jobOrderDate} , exDate:{exDate}"
                            Error = "Incorrect file",
                        };
                    }
                    //  }

                    if (checkFileExtension != zipFileExtension)
                    {
                        _logger.LogInfo($"Enter in if confotion");

                        // if (jobOrderPreviousDate == exDate || )&& jobOrderDate == exDate
                        // {
                        if (filetext == masterFileExtension && jobOrderStoreName == storeNumber || exDate == jobOrderDate)
                        {
                            _logger.LogInfo($"Check file validations");
                            var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == jobOrderStoreName && x.FileDate == jobOrderDate && x.Category == "OrderJob");
                            if (filedata.Result != null)
                            {
                                return new FileUplaodRespone
                                {
                                    Success = false,
                                    Error = "File is already uploaded."
                                };
                            }
                            // condition 
                            var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                            fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.

                            if (!Directory.Exists(pathBuilt))
                            {
                                Directory.CreateDirectory(pathBuilt);
                            }

                            var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files", fileName);
                            _logger.LogInfo($"Get path");
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                _logger.LogInfo($"stream");
                                await model.File.CopyToAsync(stream);
                            }

                            _logger.LogInfo($"After stream");
                            string regex = "^0+(?!$)";
                            var orderJobFile = File.ReadAllLines(path);

                            // for inner header
                            string jobOrderInnerFileHeader = orderJobFile[0].Substring(0, 6).Trim();
                            _logger.LogInfo($"jobOrderInnerFileHeader : {jobOrderInnerFileHeader}");
                            string jobOrderInnerFileName = orderJobFile[0].Substring(7, 11).Trim();
                            _logger.LogInfo($"jobOrderInnerFileName : {jobOrderInnerFileName}");
                            string jobOrderInnerStoreNumber = orderJobFile[0].Substring(18, 4).Trim();
                            _logger.LogInfo($"jobOrderInnerStoreNumber : {jobOrderInnerStoreNumber}");
                            string jobOrderInnerFileDate = orderJobFile[0].Substring(22, 7).Trim();
                            _logger.LogInfo($"jobOrderInnerFileDate : {jobOrderInnerFileDate}");
                            //string jobOrderInnerDate = "210920";// string.Format("{0:ddMMyy}", jobOrderInnerFileDate).Trim();
                            try
                            {
                                _logger.LogInfo($"masterFile : {masterFile} ,storeNumber : {storeNumber} , forInnerStockDate : {forInnerStockDate}");
                                // jobOrderInnerFileDate == forInnerStockDate
                                // if (jobOrderPreviousDate == exDate || jobOrderDate == exDate)

                                //if (jobOrderInnerFileDate == forInnerPreviousStockDate || forInnerStockDate == jobOrderInnerFileDate)&& forInnerStockDate == jobOrderInnerFileDate
                                //{
                                if (jobOrderInnerFileName == masterFile && jobOrderInnerStoreNumber == storeNumber && forInnerStockDate == jobOrderInnerFileDate)
                                {
                                    // file content
                                    orderJobList = orderJobFile.Skip(1).SkipLast(1).SelectMany(x => new List<OrderJob>
                                    {
                                    new()
                                    {
                                        Store = x.Substring(0,4),
                                        Code= x.Substring(4,14),
                                        Department=Regex.Replace(x.Substring(18,4),regex, ""),
                                        Description =Regex.Replace(x.Substring(22,30),regex, ""),
                                        SalePrice= Regex.Replace(x.Substring(52,8),regex, ""),
                                        PriceWithoutTaxes= Regex.Replace(x.Substring(60,8),regex, ""),
                                        SKU = Regex.Replace(x.Substring(68, 14), regex, ""),
                                        Category=x.Length  ==88 ?x.Substring(82,6):null,
                                        CustomerId = model.CustomerId,
                                        StoreId = model.StoreId,
                                        StockDate = model.StockDate,
                                    }
                                    }).ToList();
                                    _logger.LogInfo($"orderJobList data : {orderJobList}");

                                    //for TRAILER Records
                                    trailerRecords = orderJobFile.TakeLast(1).SelectMany(y => new List<FileStore>
                                        {
                                        new()
                                        {
                                            RecordCount=y.Substring(29,6),
                                        }}).ToList();

                                    _logger.LogInfo($"trailerRecords : {trailerRecords}");
                                    // Replaces the matched
                                    string str = Regex.Replace(trailerRecords[0].RecordCount, regex, "");
                                    if (str == orderJobList.Count.ToString())
                                    { }
                                    else
                                    {
                                        return new FileUplaodRespone
                                        {
                                            Success = false,
                                            Error = "TRAILER record not match."
                                        };
                                    }
                                    await _context.BulkInsertAsync(orderJobList);
                                    stopwatch.Stop();
                                    timeElapsed = Math.Ceiling(stopwatch.Elapsed.TotalSeconds);
                                    uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "OrderJob" } };
                                    await _context.BulkInsertAsync(uploadFileNames);
                                    //for file header
                                    fileStores = orderJobFile.Take(1).SelectMany(y => new List<FileStore>
                                        {
                                        new()
                                        {
                                            Header=y.Substring(0,6),
                                            FileName=y.Substring(8,7),
                                            StoreNumber=y.Substring(18,4),
                                            FileDate=exDate,
                                            Category="OrderJob",
                                        }}).ToList();
                                    await _context.BulkInsertAsync(fileStores);
                                    isSaveSuccess = true;
                                    File.Delete(path);
                                    FileStore fileStore = _context.FileStore.Where(u => u.Category == "OrderJob" && u.FileDate == tempfileName.Substring(4, 4) && u.StoreNumber == jobOrderStoreName && u.FileName == "MASTER" && u.Header == "HEADER").SingleOrDefault();
                                    fileStore.RecordCount = orderJobList.Count.ToString();
                                    fileStore.Status = "OKAY";
                                    await _context.SaveChangesAsync();
                                    return new FileUplaodRespone
                                    {
                                        stockRecordCount = orderJobList.Count.ToString(),
                                        TimeElapsed = timeElapsed,
                                        Success = isSaveSuccess
                                    };
                                }
                                //}
                                //return new FileUplaodRespone
                                //{
                                // Success = false,
                                // Error = "Previous Date not match."
                                // };
                            }
                            catch (Exception e)
                            {
                                File.Delete(path);
                                return new FileUplaodRespone
                                {
                                    Success = false,
                                    Error = "Please select correct file......"
                                };

                            }
                        }
                        // }
                    }
                }
                if (checkFileExtension == zipFileExtension)
                {
                    _logger.LogInfo("Master zip file started");
                    //for zip file
                    string existingFile = model.File.FileName.ToString();
                    _logger.LogInfo($"existingFile : {existingFile}");
                    // var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                    // fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.
                    pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");
                    _logger.LogInfo($"pathBuilt : {pathBuilt}");
                    if (!Directory.Exists(pathBuilt))
                    {
                        _logger.LogInfo($"Create directory");
                        Directory.CreateDirectory(pathBuilt);
                    }
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files",
                    existingFile);
                    _logger.LogInfo($"path : {path}");

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        _logger.LogInfo($"stream started");
                        await model.File.CopyToAsync(stream);
                    }
                    _logger.LogInfo($"stream ended");
                    string zipPath = Path.GetFileName(path);
                    _logger.LogInfo($"Extracting zip");
                    ZipFile.ExtractToDirectory(path, pathBuilt);
                    _logger.LogInfo($"Extracted zip");

                    var filePaths = Directory.GetFiles(pathBuilt, "*.txt");
                    _logger.LogInfo($"filePaths : {filePaths}");
                    string destinationPath = filePaths[0].ToString();
                    var extension = "." + destinationPath.Split('.')[model.File.FileName.Split('.').Length - 1];

                    // if (jobOrderPreviousDate == exDate) || jobOrderDate == exDate
                    // {
                    if (filetext == masterFileExtension || jobOrderStoreName == storeNumber || exDate == jobOrderDate)
                    {
                        var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == jobOrderStoreName && x.FileDate == jobOrderDate && x.Category == "OrderJob");
                        if (filedata.Result != null)
                        {
                            return new FileUplaodRespone
                            {
                                Success = false,
                                Error = "Master zip file is already uploaded."
                            };
                        }
                        var zipOrderJobFile = File.ReadAllLines(destinationPath);
                        //get the trailer records
                        trailerRecords = zipOrderJobFile.TakeLast(1).SelectMany(y => new List<FileStore>
                                        {
                                        new()
                                        {
                                            RecordCount=y.Substring(29,6),
                                        }}).ToList();
                        string regex = "^0+(?!$)";
                        string jobOrderInnerFileHeader = zipOrderJobFile[0].Substring(0, 6).Trim();
                        string jobOrderInnerFileName = zipOrderJobFile[0].Substring(7, 11).Trim();
                        string jobOrderInnerStoreNumber = zipOrderJobFile[0].Substring(18, 4).Trim();
                        string jobOrderInnerFileDate = zipOrderJobFile[0].Substring(22, 7).Trim();
                        // string jobOrderInnerDate = string.Format("{0:ddMMyy}", jobOrderInnerFileDate).Trim();
                        try
                        {
                            //if (jobOrderInnerFileDate == forInnerPreviousStockDate) || forInnerStockDate == jobOrderInnerFileDate
                            //  {
                            if (jobOrderInnerFileName == masterFile || jobOrderInnerStoreNumber == storeNumber || forInnerStockDate == jobOrderInnerFileDate)
                            {
                                orderJobList = zipOrderJobFile.Skip(1).SkipLast(1).SelectMany(x => new List<OrderJob>
                                        {
                                        new()
                                        {
                                            Store = x.Substring(0,4),
                                            Code= x.Substring(4,14),
                                            Department=Regex.Replace(x.Substring(18,4),regex, ""),
                                            Description =Regex.Replace(x.Substring(22,30),regex, ""),
                                            SalePrice= Regex.Replace(x.Substring(52,8),regex, ""),
                                            PriceWithoutTaxes= Regex.Replace(x.Substring(60,8),regex, ""),
                                            SKU = Regex.Replace(x.Substring(68, 14), regex, ""),
                                            Category=x.Length  ==88 ?x.Substring(82,6):null,
                                            CustomerId = model.CustomerId,
                                            StoreId = model.StoreId,
                                            StockDate = model.StockDate,
                                        }
                                        }).ToList();
                            }
                            // }
                            // Replaces the matched
                            string str = Regex.Replace(trailerRecords[0].RecordCount, regex, "");
                            if (str == orderJobList.Count.ToString())
                            { }
                            else
                            {
                                return new FileUplaodRespone
                                {
                                    Success = false,
                                    Error = "TRAILER record not match."
                                };
                            }

                            await _context.BulkInsertAsync(orderJobList);
                            stopwatch.Stop();
                            timeElapsed = Math.Ceiling(stopwatch.Elapsed.TotalSeconds);

                            uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "OrderJob" } };
                            await _context.BulkInsertAsync(uploadFileNames);

                            //for file header
                            fileStores = zipOrderJobFile.Take(1).SelectMany(y => new List<FileStore>
                                    {
                                    new()
                                    {
                                            Header=y.Substring(0,6),
                                            FileName=y.Substring(8,7),
                                            StoreNumber=y.Substring(18,4),
                                            FileDate=exDate,
                                            Category="OrderJob",
                                    }}).ToList();
                            await _context.BulkInsertAsync(fileStores);
                        }
                        catch (Exception e)
                        {

                            File.Delete(pathBuilt);
                            File.Delete(destinationPath);
                            return new FileUplaodRespone
                            {
                                Success = false,
                                Error = e.Message
                            };

                            // Submit the change to the database
                        }
                    }
                    // }
                    isSaveSuccess = true;
                    FileStore fileStore = _context.FileStore.Where(u => u.Category == "OrderJob" && u.FileDate == tempfileName.Substring(4, 4) && u.StoreNumber == jobOrderStoreName && u.FileName == "MASTER" && u.Header == "HEADER").SingleOrDefault();
                    fileStore.RecordCount = orderJobList.Count.ToString();
                    fileStore.Status = "OKAY";
                    await _context.SaveChangesAsync();

                    return new FileUplaodRespone
                    {
                        stockRecordCount = orderJobList.Count.ToString(),
                        TimeElapsed = timeElapsed,
                        Success = isSaveSuccess
                    };
                }
            }

            catch (Exception e)
            {
                return new FileUplaodRespone
                {
                    Success = false,
                    Error = e.Message
                };
            }
            return new FileUplaodRespone
            {
                Success = false,
                //  Error = $"Some errors occurred for mismatch date.! forInnerStockDate : {forInnerStockDate} , innerDataError : {innerDataError}"
                Error = "Some errors occurred for mismatch date.",//! forInnerStockDate : {forInnerStockDate} , innerDataError : {innerDataError}"
            };
        }
        public async Task<FileUplaodRespone> DepartmentsData(FilterDataModel model)
        {
            TimeZoneInfo localZones = TimeZoneInfo.Local;
            DateTime currentDate = model.StockDate.Value;
            _logger.LogInfo($"Get DepartmentsDate : {currentDate}");

            string regex = "^0+(?!$)";
            _logger.LogInfo("DepartmentsData method started");
            bool isSaveSuccess = false;
            string fileName;
            var innerDataError = model.StockDate.Value.Date.AddDays(-1).Day.ToString("#00") + model.StockDate.Value.Month.ToString("#00") +
              model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();

            _logger.LogInfo("innerDataError for check error");

            var finaldeptartmentDate = model.StockDate.Value.Date.Month.ToString("#00") +
              model.StockDate.Value.Date.AddDays(-1).Day.ToString("#00");

            var departmentPreviousDate = model.StockDate.Value.Date.Month.ToString("#00") +
                    model.StockDate.Value.Date.AddDays(-2).Day.ToString("#00");
            var forInnerdeptartmentDate = string.Empty;

            var forInnerPreviousdeptartmentDat = model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString()
                    + model.StockDate.Value.Date.Month.ToString("#00")
                    + model.StockDate.Value.Date.AddDays(-2).Day.ToString("#00");
            List<Departments> departmentsList = new();
            //for file header
            List<FileStore> fileDepartment = new();
            List<FileStore> trailerRecords = new();
            //for file name
            List<UploadFileName> uploadFileNames = new();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");
            _logger.LogInfo("get pathBuilt");
            if (Directory.Exists(pathBuilt))
            {
                DirectoryInfo di = new DirectoryInfo(pathBuilt);
                FileInfo[] files = di.GetFiles();
                foreach (FileInfo file in files)
                {
                    file.Delete();
                }
            }
            double timeElapsed = 0;
            try
            {
                var deptartmentDate = model.StockDate.ToString();
                var deptartmentStore = model.StoreId.ToString();
                var deptartmentStoreName = model.StoreName.ToString().Substring(0, 4);
                //  var forInnerStockDates = Convert.ToDateTime("35-13-2029");
                _logger.LogInfo($"Get finaldeptartmentDate : {finaldeptartmentDate}");
                try
                {
                    forInnerdeptartmentDate = currentDate.Date.Year.ToString().Substring(2, 2).ToString()
                        + currentDate.Date.Month.ToString("#00") 
                        + currentDate.Date.AddDays(-1).Day.ToString("#00");
                    _logger.LogInfo($"Get forInnerdeptartmentDate : {forInnerdeptartmentDate}");
                }
                catch (Exception ex)
                {
                    return new FileUplaodRespone
                    {
                        Success = false,
                        Error = "Inner DepartmentDate error"
                    };
                }

                var pathFile = model.File.FileName;
                _logger.LogInfo($"Get pathFile : {pathFile}");
                var tempfileName = new DirectoryInfo(pathFile).Name;
                var filetext = tempfileName.Substring(0, 4);
                var exDate = tempfileName.Substring(4, 4);
                var storeNumber = tempfileName.Substring(9, 4);
                var checkFileExtension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];

                _logger.LogInfo($"checkFileExtension : {checkFileExtension}");
                if (checkFileExtension == txtFileExtension || checkFileExtension == zipFileExtension)
                {
                    _logger.LogInfo($"filetext : {filetext}");
                    _logger.LogInfo($"deptFileExtension : {deptFileExtension}");
                    _logger.LogInfo($"deptartmentStoreName : {deptartmentStoreName}");
                    _logger.LogInfo($"storeNumber : {storeNumber}");
                    _logger.LogInfo($"finaldeptartmentDate : {finaldeptartmentDate}");
                    _logger.LogInfo($"exDate : {exDate}");
                    _logger.LogInfo($"finaldeptartmentDate: {finaldeptartmentDate}");
                    _logger.LogInfo($"exDateDepartment:{exDate}");
                    _logger.LogInfo($"finalDepepartment:{finaldeptartmentDate }");

                    //if (departmentPreviousDate == exDate || finaldeptartmentDate == exDate)
                    // {

                    if (filetext != deptFileExtension && deptartmentStoreName != storeNumber && exDate!= finaldeptartmentDate)
                    {
                            return new FileUplaodRespone
                            {
                                Success = false,
                                Error = $"Incorrect file with tempfileName : {tempfileName} , filetext:{filetext} , departmentFileExtension:{deptFileExtension} , departmentStoreName:{deptartmentStoreName} , storeNumber:{storeNumber} , departmentDate:{finaldeptartmentDate} , exDate:{exDate}"
                            };
                        }
                   // }

                    if (checkFileExtension != zipFileExtension)
                    {
                        _logger.LogInfo($"Enter in if (checkFileExtension) ");

                       // if (departmentPreviousDate == exDate || finaldeptartmentDate == exDate)
                       // {
                            if (filetext == deptFileExtension && deptartmentStoreName == storeNumber && finaldeptartmentDate == exDate)
                            {
                                var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == deptartmentStoreName && x.FileDate == finaldeptartmentDate && x.Category == "DPTO");
                                if (filedata.Result != null)
                                {
                                    return new FileUplaodRespone
                                    {
                                        Success = false,
                                        Error = "File is already uploaded."
                                    };
                                }
                                var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                                fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.
                                if (!Directory.Exists(pathBuilt))
                                {
                                    Directory.CreateDirectory(pathBuilt);
                                }
                                var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files", fileName);
                            _logger.LogInfo($"Getpath");
                                using (var stream = new FileStream(path, FileMode.Create))
                                {
                                _logger.LogInfo($"stream");
                                    await model.File.CopyToAsync(stream);
                                }
                                var deptFileData = File.ReadAllLines(path);

                                // for TRAILER
                                trailerRecords = deptFileData.TakeLast(1).SelectMany(y => new List<FileStore>
                                    {
                                    new()
                                    {
                                        RecordCount=y.Substring(29,6),
                                    }}).ToList();

                            //for the trailor
                            _logger.LogInfo($"After stream");

                                string deptInnerFileHeader = deptFileData[0].Substring(0, 6).Trim();
                                string deptInnerFileName = deptFileData[0].Substring(7, 9).Trim();
                                string deptInnerStoreNumber = deptFileData[0].Substring(18, 4).Trim();
                                string deptInnerFileDate = deptFileData[0].Substring(22, 7).Trim();
                                try
                                {
                                _logger.LogInfo($"departmentFile : {deptFile} ,storeNumber : {storeNumber} , forInnerStockDate : {forInnerdeptartmentDate}");

                                   // if (deptInnerFileDate == forInnerPreviousdeptartmentDat || deptInnerFileDate== forInnerdeptartmentDate)
                                   // {
                                            if (deptInnerFileName == deptFile && deptInnerStoreNumber == storeNumber && forInnerdeptartmentDate== deptInnerFileDate)
                                            {
                                                departmentsList = deptFileData.Skip(1).SkipLast(1).SelectMany(x => new List<Departments>
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
                                            }
                                    //}
                                   
                                    //check the trailer validation
                                    string records = Regex.Replace(trailerRecords[0].RecordCount, regex, "");
                                    if (records == departmentsList.Count.ToString())
                                    { }
                                    else
                                    {
                                        return new FileUplaodRespone
                                        {
                                            Success = false,
                                            Error = "TRAILER record not match."
                                        };
                                    }
                                    await _context.BulkInsertAsync(departmentsList);
                                    stopwatch.Stop();
                                    timeElapsed = Math.Ceiling(stopwatch.Elapsed.TotalSeconds);
                                    uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "DPTO" } };
                                    await _context.BulkInsertAsync(uploadFileNames);
                                    //for file header
                                    fileDepartment = deptFileData.Take(1).SelectMany(y => new List<FileStore>
                                    {
                                    new()
                                    {
                                        Header=y.Substring(0,6),
                                        FileName=y.Substring(8,8),
                                        StoreNumber=y.Substring(18,4),
                                        FileDate=exDate,
                                        Category="DPTO",
                                    }}).ToList();
                                    await _context.BulkInsertAsync(fileDepartment);


                                    isSaveSuccess = true;
                                    File.Delete(path);
                                    FileStore fileStore = _context.FileStore.Where(u => u.Category == "DPTO"  && u.FileDate== tempfileName.Substring(4, 4) && u.StoreNumber == deptartmentStoreName && u.Header == "HEADER" && u.FileName == "DPTO DIV").SingleOrDefault();
                                    fileStore.RecordCount = departmentsList.Count.ToString();
                                    fileStore.Status = "OKAY";
                                    await _context.SaveChangesAsync();
                                    return new FileUplaodRespone
                                    {
                                        stockRecordCount = departmentsList.Count.ToString(),
                                        TimeElapsed = timeElapsed,
                                        Success = isSaveSuccess
                                    };
                                }
                                catch (Exception e)
                                {
                                    File.Delete(path);
                                    return new FileUplaodRespone
                                    {
                                        Success = false,
                                        Error = "Please select correct file......"
                                    };
                                }
                            }
                       // }
                        //return new FileUplaodRespone
                       // {
                        //    Success = false,
                        //    Error = "Previous Date not match."
                        //};
                    }
                }
                if (checkFileExtension == zipFileExtension)
                {
                    //for zip file
                    string existingFile = model.File.FileName.ToString();

                    // var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                    // fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.
                    pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");
                    if (!Directory.Exists(pathBuilt))
                    {
                        Directory.CreateDirectory(pathBuilt);
                    }
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files",
                    existingFile);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.File.CopyToAsync(stream);
                    }
                    string zipPath = Path.GetFileName(path);
                    ZipFile.ExtractToDirectory(path, pathBuilt);
                    var filePaths = Directory.GetFiles(pathBuilt, "*.txt");
                    string destinationPath = filePaths[0].ToString();
                    var extension = "." + destinationPath.Split('.')[model.File.FileName.Split('.').Length - 1];
                    _logger.LogInfo($" second DepartmentfinaldeptartmentDate:{finaldeptartmentDate }");
                    _logger.LogInfo($"exDate Department:{exDate }");

                    // if (departmentPreviousDate == exDate || finaldeptartmentDate == exDate)
                    // {

                    if (filetext == deptFileExtension && deptartmentStoreName == storeNumber && finaldeptartmentDate == exDate)
                        {
                            var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == deptartmentStoreName && x.FileDate == finaldeptartmentDate && x.Category == "DPTO");
                            if (filedata.Result != null)
                            {
                                return new FileUplaodRespone
                                {
                                    Success = false,
                                    Error = "Department zip file is already uploaded."
                                };
                            }
                            var zipDepartmentFile = File.ReadAllLines(destinationPath);
                            // get trailer records
                            trailerRecords = zipDepartmentFile.TakeLast(1).SelectMany(y => new List<FileStore>
                                    {
                                    new()
                                    {
                                        RecordCount=y.Substring(29,6),
                                    }}).ToList();

                            await _context.BulkInsertAsync(fileDepartment);
                            string deptInnerFileHeader = zipDepartmentFile[0].Substring(0, 6).Trim();
                            string deptInnerFileName = zipDepartmentFile[0].Substring(7, 5).Trim();
                            string deptInnerStoreNumber = zipDepartmentFile[0].Substring(18, 4).Trim();
                            string deptInnerFileDate = zipDepartmentFile[0].Substring(22, 7).Trim();
                            // string stockInnerDate = string.Format("{0:ddMMyy}", stockInnerFileDate).Trim();
                            try
                            {
                            _logger.LogInfo($"second DepartmentdeptInnerFileDate:{deptInnerFileDate}");
                            _logger.LogInfo($" Department finaldeptartmentDate: {finaldeptartmentDate }");

                            // || deptInnerFileDate == finaldeptartmentDate
                            if (deptInnerFileName == deptFileExtension || deptInnerStoreNumber == storeNumber || deptInnerFileDate == finaldeptartmentDate)
                                {
                                    departmentsList = zipDepartmentFile.Skip(1).SkipLast(1).SelectMany(x => new List<Departments>
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
                                }
                                //check the trailer validation
                                string records = Regex.Replace(trailerRecords[0].RecordCount, regex, "");
                                if (records == departmentsList.Count.ToString())
                                { }
                                else
                                {
                                    return new FileUplaodRespone
                                    {
                                        Success = false,
                                        Error = "TRAILER record not match."
                                    };
                                }
                                await _context.BulkInsertAsync(departmentsList);
                                stopwatch.Stop();
                                timeElapsed = Math.Ceiling(stopwatch.Elapsed.TotalSeconds);
                                uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = deptartmentStoreName, FileDate = tempfileName.Substring(4, 4), Category = "DPTO" } };
                                await _context.BulkInsertAsync(uploadFileNames);

                                fileDepartment = zipDepartmentFile.Take(1).SelectMany(y => new List<FileStore>
                                {
                                new()
                                {
                                Header=y.Substring(0,6),
                                FileName=y.Substring(8,8),
                                StoreNumber=y.Substring(18,4),
                                FileDate=exDate,
                                Category="DPTO",
                                }}).ToList();
                                await _context.BulkInsertAsync(fileDepartment);
                            }
                            catch (Exception e)
                            {

                                File.Delete(pathBuilt);
                                File.Delete(destinationPath);
                                return new FileUplaodRespone
                                {
                                    Success = false,
                                    Error = e.Message
                                };
                            }
                        }
                   // }
                    isSaveSuccess = true;
                    FileStore fileStore = _context.FileStore.Where(u => u.Category == "DPTO"  && u.FileDate== tempfileName.Substring(4, 4) && u.StoreNumber == deptartmentStoreName && u.Header == "HEADER" && u.FileName == "DPTO DIV").SingleOrDefault();
                    fileStore.RecordCount = departmentsList.Count.ToString();
                    fileStore.Status = "OKAY";
                    await _context.SaveChangesAsync();

                    return new FileUplaodRespone
                    {
                        stockRecordCount = departmentsList.Count.ToString(),
                        TimeElapsed = timeElapsed,
                        Success = isSaveSuccess
                    };
                }
            }
            catch (Exception e)
            {
                return new FileUplaodRespone
                {
                    Success = false,
                    Error = e.Message
                };
            }
            return new FileUplaodRespone
            {
                Success = false,
                // Error = $"Some errors occurred! forInnerdeptartmentDate : {forInnerdeptartmentDate} , innerDataError : {innerDataError}"
                Error = "Some errors occurred for mismatch date!",
            };
        }
        public async Task<FileUplaodRespone> ReservedData(FilterDataModel model)
        {
            TimeZoneInfo localZones = TimeZoneInfo.Local;
            DateTime currentDate = model.StockDate.Value;
            _logger.LogInfo($"Get ReservedDate : {currentDate}");

            string regex = "^0+(?!$)";
            bool isSaveSuccess = false;
            string fileName;
            List<Reserved> reservedList = new();
            List<StockAdjustment> stockAdjustmentList = new();
            //for file header
            List<FileStore> fileStores = new();
            List<FileStore> trailerRecords = new();
            //for file name
            List<UploadFileName> uploadFileNames = new();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");

            if (Directory.Exists(pathBuilt))
            {
                DirectoryInfo di = new DirectoryInfo(pathBuilt);
                FileInfo[] files = di.GetFiles();
                foreach (FileInfo file in files)
                {
                    file.Delete();
                }
            }
            double timeElapsed = 0;
            try
            {
                var reservedDate = model.StockDate.ToString();
                var reservedStore = model.StoreId.ToString();
                var reservedStoreName = model.StoreName.ToString().Substring(0, 4);

                var finalReservedDate = model.StockDate.Value.Date.Month.ToString("#00") +
                   model.StockDate.Value.Date.AddDays(-1).Day.ToString("#00");
                _logger.LogInfo($"finalReservedDate : {finalReservedDate}");
                var ReservedPreviousDate = model.StockDate.Value.Date.Month.ToString("#00") +
                model.StockDate.Value.Date.AddDays(-2).Day.ToString("#00");
                _logger.LogInfo($"ReservedPreviousDate : {ReservedPreviousDate}");
                var ReservedPreviousInnerDate = model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString()
                       + model.StockDate.Value.Date.Month.ToString("#00")
                       + model.StockDate.Value.Date.AddDays(-2).Day.ToString("#00");
                _logger.LogInfo($"ReservedPreviousInnerDate : {ReservedPreviousInnerDate}");
                //  var forInnerStockDates = Convert.ToDateTime("35-13-2029");
                var finalInnerreservedDate = string.Empty;
                try
                {
                    finalInnerreservedDate = model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString()
                         + model.StockDate.Value.Date.Month.ToString("#00")
                         + model.StockDate.Value.Date.AddDays(-1).Day.ToString("#00");
                    _logger.LogInfo($"finalInnerreservedDate : {finalInnerreservedDate}");
                }
                catch (Exception ex)
                {
                    return new FileUplaodRespone
                    {
                        Success = false,
                        Error = "Inner Reserved Date error"
                    };
                }
                var pathFile = model.File.FileName;
                _logger.LogInfo($"Get pathFile : {pathFile}");
                var tempfileName = new DirectoryInfo(pathFile).Name;
             
                var filetext = tempfileName.Substring(0, 4);
                _logger.LogInfo($"filetext : {filetext}");
               
                var exDate = tempfileName.Substring(4, 4);
                _logger.LogInfo($"exDate : {exDate}");
                var storeNumber = tempfileName.Substring(9, 4);
                _logger.LogInfo($"storeNumber : {storeNumber}");

                var checkFileExtension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                if (checkFileExtension == txtFileExtension || checkFileExtension == zipFileExtension)
                {
                    _logger.LogInfo($"finalReservedDate : {finalReservedDate}");
                    _logger.LogInfo($"exDate : {exDate}");
                    _logger.LogInfo($"finalReservedDate : {finalReservedDate}");

                    // if (ReservedPreviousDate == exDate || finalReservedDate == exDate)
                    // {
                    if (filetext != reservedFileExtension || reservedStoreName != storeNumber || finalReservedDate!=exDate)
                    {
                            return new FileUplaodRespone
                            {
                                Success = false,
                                Error = "Please choose correct file."
                            };
                        }
                   // }
                    if (checkFileExtension != zipFileExtension)
                    {
                        //  if (ReservedPreviousDate == exDate || finalReservedDate == exDate)
                        // {

                        _logger.LogInfo($"reservedStoreName : {reservedStoreName}");
                        if (filetext == reservedFileExtension && reservedStoreName == storeNumber && finalReservedDate == exDate)
                            {
                                var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == reservedStoreName && x.FileDate == finalReservedDate && x.Category == "Reserved");
                                if (filedata.Result != null)
                                {
                                    return new FileUplaodRespone
                                    {
                                        Success = false,
                                        Error = "File is already uploaded."
                                    };
                                }
                                // condition 

                                var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                                fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.

                                if (!Directory.Exists(pathBuilt))
                                {
                                    Directory.CreateDirectory(pathBuilt);
                                }

                                var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files", fileName);
                                using (var stream = new FileStream(path, FileMode.Create))
                                {
                                    await model.File.CopyToAsync(stream);
                                }
                                var ReservedFile = File.ReadAllLines(path);


                                // get the trailer records 
                                trailerRecords = ReservedFile.TakeLast(1).SelectMany(y => new List<FileStore>
                            {
                            new()
                            {
                            RecordCount=y.Substring(29,6),
                            }}).ToList();

                                // for inner header
                                string reservedInnerFileHeader = ReservedFile[0].Substring(0, 6).Trim();
                                string reservedInnerFileName = ReservedFile[0].Substring(7, 5).Trim();
                                string reservedInnerStoreNumber = ReservedFile[0].Substring(18, 4).Trim();
                                string reservedInnerFileDate = ReservedFile[0].Substring(22, 7).Trim();

                                //string jobOrderInnerDate = "210920";// string.Format("{0:ddMMyy}", jobOrderInnerFileDate).Trim();
                                try
                                {
                                _logger.LogInfo($"reservedInnerStoreNumber : {reservedInnerStoreNumber}");

                                //if (ReservedPreviousInnerDate == reservedInnerFileDate|| finalInnerreservedDate== reservedInnerFileDate)
                                // {
                                if (reservedInnerFileName == reservedFileExtension && reservedInnerStoreNumber == storeNumber && finalInnerreservedDate== reservedInnerFileDate)
                                     {
                                            reservedList = ReservedFile.Skip(1).SkipLast(1).SelectMany(x => new List<Reserved>
                                            {
                                            new()
                                            {
                                                Store=x.Substring(0,4),
                                                Code=x.Substring(4,14),
                                                Quantity=Regex.Replace(x.Substring(18,9),regex,""),
                                                Filler=Regex.Replace(x.Substring(27,1),regex,""),
                                                CustomerId = model.CustomerId,
                                                StoreId = model.StoreId,
                                                StockDate = model.StockDate,
                                            }
                                            }).ToList();

                                            // Replaces the matched
                                            string str = Regex.Replace(trailerRecords[0].RecordCount, regex, "");
                                            if (str == reservedList.Count.ToString())
                                            { }
                                            else
                                            {
                                                return new FileUplaodRespone
                                                {
                                                    Success = false,
                                                    Error = "TRAILER record not match."
                                                };
                                            }
                                            await _context.BulkInsertAsync(reservedList);
                                            //   var customerId = new SqlParameter("@customerId", model.CustomerId);
                                            //   await _context.StockAdjustment.FromSqlRaw("EXECUTE dbo.AutoAppendReservedRecords {0}", customerId).ToListAsync();
                                            var query = (from b in _context.OrderJob
                                                         join a in _context.Reserved on b.Code equals a.Code
                                                         select new ReservedAndOrderModel
                                                         {
                                                             SKU = b.Id,
                                                             barCode = a.Code,
                                                             NOF = false,
                                                             Isactive = false,
                                                             Isdeleted = false,
                                                             Tag = "3001",
                                                             Qty = Convert.ToDouble(a.Quantity),
                                                             Shelf = "01",
                                                             Date = a.CreatedAt,
                                                             Department = b.Department,
                                                         }).ToList();

                                            stockAdjustmentList = query.SelectMany(x => new List<StockAdjustment>
                                    {
                                    new()
                                    {
                                        Rec=0,
                                        SKU=x.SKU,
                                        Barcode=x.barCode,
                                        NOF=0,
                                        Isdeleted=x.Isdeleted,
                                        Tag=Convert.ToInt32(x.Tag),
                                        Shelf=Convert.ToInt32(x.Shelf),
                                        Quantity=(int?)x.Qty,
                                        CreatedAt=x.Date,
                                        Dload=1,
                                        Term="99999",
                                        Department=Convert.ToInt32(x.Department),
                                    }
                                    }).ToList();

                                            await _context.BulkInsertAsync(stockAdjustmentList);

                                            stockAdjustmentList = new List<StockAdjustment>();

                                            stockAdjustmentList = await _context.StockAdjustment.ToListAsync();

                                            foreach (var item in stockAdjustmentList)
                                            {
                                                item.Rec = SetRecNumber();
                                                await _context.SingleUpdateAsync(item);
                                            }

                                            //  await _context.Reserved.FromSqlRaw(@"exec AutoAppendReservedRecords").ToListAsync();
                                            // var results = _context.Reserved.FromSqlRaw("INSERT INTO StockAdjustment (Id, Barcode, IsActive, SKU, NOF, Isdeleted, Tag, Shelf, Quantity, CreatedAt) SELECT  NEWID() AS Id, Reserved.Code, Reserved.IsActive, OrderJob.Id AS JobOrderId, 0 AS nof, 0 AS isDeleted, 3001 AS Tag, 01 AS shelf, CAST(CAST(Reserved.Quantity AS NUMERIC) AS INT) AS Qty, { fn NOW() } AS Date FROM Reserved INNER JOIN OrderJob ON Reserved.Code = OrderJob.Code");

                                            stopwatch.Stop();
                                            timeElapsed = Math.Ceiling(stopwatch.Elapsed.TotalSeconds);
                                            uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "Reserved" } };
                                            await _context.BulkInsertAsync(uploadFileNames);

                                            //for file header
                                            fileStores = ReservedFile.Take(1).SelectMany(y => new List<FileStore>
                                        {
                                        new()
                                        {
                                            Header=y.Substring(0,6),
                                            FileName=y.Substring(8,9),
                                            StoreNumber=y.Substring(18,4),
                                            FileDate=exDate,
                                            Category="Reserved",
                                            }}).ToList();
                                            await _context.BulkInsertAsync(fileStores);
                                            isSaveSuccess = true;
                                            File.Delete(path);
                                            FileStore fileStore = _context.FileStore.Where(u => u.Category == "Reserved" && u.FileDate== tempfileName.Substring(4, 4) && u.StoreNumber == reservedStoreName && u.FileName == "APARTADOS" && u.Header == "HEADER").SingleOrDefault();
                                            fileStore.RecordCount = reservedList.Count.ToString();
                                            fileStore.Status = "OKAY";
                                            await _context.SaveChangesAsync();
                                            return new FileUplaodRespone
                                            {
                                                stockRecordCount = reservedList.Count.ToString(),
                                                TimeElapsed = timeElapsed,
                                                Success = isSaveSuccess
                                            };
                                        }
                                    //}

                                    //return new FileUplaodRespone
                                    //{
                                    //    Success = false,
                                    //    Error = "Previous Date not match."
                                    //};
                                }
                                catch (Exception e)
                                {
                                    File.Delete(path);
                                    return new FileUplaodRespone
                                    {
                                        Success = false,
                                        Error = "Please select correct file......"
                                    };

                                }
                            }
                        //}
                        //return new FileUplaodRespone
                        //{
                        //    Success = false,
                        //    Error = "Previous Date not match."
                        //};
                    }
                }
                if (checkFileExtension == zipFileExtension)
                {
                    //for zip file
                    string existingFile = model.File.FileName.ToString();

                    // var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                    // fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.
                    pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");
                    if (!Directory.Exists(pathBuilt))
                    {
                        Directory.CreateDirectory(pathBuilt);
                    }
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files",
                    existingFile);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.File.CopyToAsync(stream);
                    }

                    string zipPath = Path.GetFileName(path);
                    ZipFile.ExtractToDirectory(path, pathBuilt);

                    var filePaths = Directory.GetFiles(pathBuilt, "*.txt");

                    string destinationPath = filePaths[0].ToString();
                    var extension = "." + destinationPath.Split('.')[model.File.FileName.Split('.').Length - 1];
                    // if (ReservedPreviousDate == exDate || finalReservedDate == exDate)
                    //{

                    _logger.LogInfo($"reservedStoreName : {reservedStoreName}");
                    _logger.LogInfo($"finalReservedDate : {finalReservedDate}");
                    if (filetext == reservedFileExtension || reservedStoreName == storeNumber || finalReservedDate == exDate)
                        {
                            var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == reservedStoreName && x.FileDate == finalReservedDate && x.Category == "Reserved");
                            if (filedata.Result != null)
                            {
                                return new FileUplaodRespone
                                {
                                    Success = false,
                                    Error = "Reserved zip file is already uploaded."
                                };
                            }
                            var zipReservedFile = File.ReadAllLines(destinationPath);
                            // get the trailer records 
                            trailerRecords = zipReservedFile.TakeLast(1).SelectMany(y => new List<FileStore>
                            {
                            new()
                            {
                            RecordCount=y.Substring(29,6),
                            }}).ToList();
                            await _context.BulkInsertAsync(fileStores);
                            string jobOrderInnerFileHeader = zipReservedFile[0].Substring(0, 6).Trim();
                            string jobOrderInnerFileName = zipReservedFile[0].Substring(7, 11).Trim();
                            string jobOrderInnerStoreNumber = zipReservedFile[0].Substring(18, 4).Trim();
                            string jobOrderInnerFileDate = zipReservedFile[0].Substring(22, 7).Trim();
                            // string jobOrderInnerDate = string.Format("{0:ddMMyy}", jobOrderInnerFileDate).Trim();
                            try
                            {
                            _logger.LogInfo($"jobOrderInnerFileDate : {jobOrderInnerFileDate}");
                            // || jobOrderInnerFileDate == finalInnerreservedDate
                            if (jobOrderInnerFileName == reservedFile || jobOrderInnerStoreNumber == storeNumber || jobOrderInnerFileDate== finalInnerreservedDate)
                            {
                                    reservedList = zipReservedFile.Skip(1).SkipLast(1).SelectMany(x => new List<Reserved>
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
                                }

                                // Replaces the matched
                                string str = Regex.Replace(trailerRecords[0].RecordCount, regex, "");
                                if (str == reservedList.Count.ToString())
                                { }
                                else
                                {
                                    return new FileUplaodRespone
                                    {
                                        Success = false,
                                        Error = "TRAILER record not match."
                                    };
                                }
                                await _context.BulkInsertAsync(reservedList);
                                stopwatch.Stop();
                                timeElapsed = Math.Ceiling(stopwatch.Elapsed.TotalSeconds);

                                uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "Reserved" } };
                                await _context.BulkInsertAsync(uploadFileNames);

                                //for file header
                                fileStores = zipReservedFile.Take(1).SelectMany(y => new List<FileStore>
                                    {
                                    new()
                                    {
                                            Header=y.Substring(0,6),
                                            FileName=y.Substring(8,9),
                                            StoreNumber=y.Substring(18,4),
                                            FileDate=exDate,
                                            Category="Reserved",
                                    }}).ToList();
                                await _context.BulkInsertAsync(fileStores);
                            }
                            catch (Exception e)
                            {

                                File.Delete(pathBuilt);
                                File.Delete(destinationPath);
                                return new FileUplaodRespone
                                {
                                    Success = false,
                                    Error = e.Message
                                };

                                // Submit the change to the database
                            }
                        }
                   // }

                    isSaveSuccess = true;
                    FileStore fileStore = _context.FileStore.Where(u => u.Category == "Reserved" && u.FileDate== tempfileName.Substring(4, 4) && u.StoreNumber == reservedStoreName && u.FileName == "APARTADOS" && u.Header == "HEADER").SingleOrDefault();
                    fileStore.RecordCount = reservedList.Count.ToString();
                    fileStore.Status = "OKAY";
                    await _context.SaveChangesAsync();

                    return new FileUplaodRespone
                    {
                        stockRecordCount = reservedList.Count.ToString(),
                        TimeElapsed = timeElapsed,
                        Success = isSaveSuccess
                    };
                }
            }

            catch (Exception e)
            {
                return new FileUplaodRespone
                {
                    Success = false,
                    Error = e.Message
                };
            }
            return new FileUplaodRespone
            {
                Success = false,
                Error = "Some errors occurred!"
            };
        }
        private int SetRecNumber()
        {
            int maxLength = (int)_context.StockAdjustment.Select(x => x.Rec).Max();
            if (maxLength == 0)
            {
              return 1;
            }
            else
            {
                return maxLength + 1;
            }
        }
        public async Task<FileUplaodRespone> StockData(FilterDataModel model)
        {
            TimeZoneInfo localZones = TimeZoneInfo.Local;
            DateTime currentDate = model.StockDate.Value;
            _logger.LogInfo($"Get StockDataDate : {currentDate}");

            string regex = "^0+(?!$)";
            bool isSaveSuccess = false;
            string fileName;
            var innerDataError = model.StockDate.Value.Date.AddDays(-1).Day.ToString("#00") + model.StockDate.Value.Date.Month.ToString("#00") +
                model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
            _logger.LogInfo($"Get innerDataError : {innerDataError}");


            List<Stock> stockList = new();

            //for file header
            List<FileStore> fileStores = new();
            List<FileStore> trailerRecords = new();
            //for file name
            List<UploadFileName> uploadFileNames = new();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");

            var StockDataPreviousDate = model.StockDate.Value.Date.Month.ToString("#00") +
                              model.StockDate.Value.Date.AddDays(-2).Day.ToString("#00");
            _logger.LogInfo($"StockDataPreviousDate : {StockDataPreviousDate}");
            var StockDataPreviousInnerDate = model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString()
                      + model.StockDate.Value.Date.Month.ToString("#00")
                      + model.StockDate.Value.Date.AddDays(-2).Day.ToString("#00");
            _logger.LogInfo($"StockDataPreviousInnerDate : {StockDataPreviousInnerDate}");
            if (Directory.Exists(pathBuilt))
            {
                DirectoryInfo di = new DirectoryInfo(pathBuilt);
                FileInfo[] files = di.GetFiles();
                foreach (FileInfo file in files)
                {
                    file.Delete();
                }
            }

            double timeElapsed = 0;
            try
            {
                var stockDate = model.StockDate.ToString();
                var stockStore = model.StoreId.ToString();
                var stockStoreName = model.StoreName.ToString().Substring(0, 4);

                var finalStockDate = model.StockDate.Value.Date.Month.ToString("#00") +
                   model.StockDate.Value.Date.AddDays(-1).Day.ToString("#00");
                //  var forInnerStockDates = Convert.ToDateTime("35-13-2029");

                _logger.LogInfo($"Get StockData : {finalStockDate}");

                var forInnerStockDate = string.Empty;
                try
                {
                    forInnerStockDate = model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString()
                         + model.StockDate.Value.Date.Month.ToString("#00")
                         + model.StockDate.Value.Date.AddDays(-1).Day.ToString("#00");
                    _logger.LogInfo($"forInnerStockDate : {forInnerStockDate}");
                }
                catch (Exception ex)
                {
                    return new FileUplaodRespone
                    {
                        Success = false,
                        Error = "Inner Stock Date error"
                    };
                }
                var pathFile = model.File.FileName;
                _logger.LogInfo($"Get pathFile : {pathFile}");
                var tempfileName = new DirectoryInfo(pathFile).Name;
                _logger.LogInfo($"Get tempfileName : {tempfileName}");
                var filetext = tempfileName.Substring(0, 4);
                var exDate = tempfileName.Substring(4, 4);
                _logger.LogInfo($"exDate : {exDate}");
                var storeNumber = tempfileName.Substring(9, 4);
                _logger.LogInfo($"storeNumber : {storeNumber}");
                var checkFileExtension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];

                if (checkFileExtension == txtFileExtension || checkFileExtension == zipFileExtension)
                {
                    _logger.LogInfo($"exDate : {exDate}");
                    // if (StockDataPreviousDate == exDate || finalStockDate == exDate)
                    // {
                    if (filetext != stockFileExtension || stockStoreName != storeNumber || exDate!=finalStockDate)
                        {
                            return new FileUplaodRespone
                            {
                                Success = false,
                                Error = "Please choose correct file."
                            };
                        }
                   // }

                    if (checkFileExtension != zipFileExtension)
                    {
                       // if (StockDataPreviousDate == exDate || finalStockDate == exDate)
                       // {
                        if (filetext == stockFileExtension && stockStoreName == storeNumber || exDate == finalStockDate)
                        {
                            var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == stockStoreName && x.FileDate == finalStockDate && x.Category == "Stock");
                            if (filedata.Result != null)
                            {
                                return new FileUplaodRespone
                                {
                                    Success = false,
                                    Error = "File is already uploaded."
                                };
                            }
                            // condition 

                            var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                            fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.

                            if (!Directory.Exists(pathBuilt))
                            {
                                Directory.CreateDirectory(pathBuilt);
                            }

                            var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await model.File.CopyToAsync(stream);
                            }
                            var stockFileData = File.ReadAllLines(path);
                            // get the trailer records 
                            trailerRecords = stockFileData.TakeLast(1).SelectMany(y => new List<FileStore>
                            {
                            new()
                            {
                            RecordCount=y.Substring(29,6),
                            }}).ToList();

                            // for inner header
                            string stockInnerFileHeader = stockFileData[0].Substring(0, 6).Trim();
                            string stockInnerFileName = stockFileData[0].Substring(7, 11).Trim();
                            string stockInnerStoreNumber = stockFileData[0].Substring(18, 4).Trim();
                            string stockInnerFileDate = stockFileData[0].Substring(22, 7).Trim();
                            //string jobOrderInnerDate = "210920";// string.Format("{0:ddMMyy}", jobOrderInnerFileDate).Trim();
                            try
                            {
                                  //  if (stockInnerFileDate == StockDataPreviousInnerDate || stockInnerFileDate == forInnerStockDate)
                                  //  {
                                        if (stockInnerFileName == stockFile && stockInnerStoreNumber == storeNumber && stockInnerFileDate == forInnerStockDate)
                                        {
                                            // file content
                                            stockList = stockFileData.Skip(1).SkipLast(1).SelectMany(x => new List<Stock>
                                                {
                                                new()
                                                {
                                                Store=(x.Substring(0,4)),
                                                SKU=Regex.Replace(x.Substring(4,14),regex,""),
                                                Department=Regex.Replace(x.Substring(18,4),regex, ""),
                                                Description = Regex.Replace(x.Substring(22, 30), regex, ""),
                                                PrecVtaNorm=Regex.Replace(x.Substring(52,8),regex,""),
                                                PrecVtaNorm_SImpto=Regex.Replace(x.Substring(60,8),regex,""),
                                                SOH=Regex.Replace(x.Substring(68,12),regex,""),
                                                Category=x.Length ==87 ?x.Substring(81,6):null,
                                                CustomerId = model.CustomerId,
                                                StoreId = model.StoreId,
                                                StockDate = currentDate,
                                                }
                                                }).ToList();

                                            // Replaces the matched
                                            string str = Regex.Replace(trailerRecords[0].RecordCount, regex, "");
                                            if (str == stockList.Count.ToString())
                                            { }
                                            else
                                            {
                                                return new FileUplaodRespone
                                                {
                                                    Success = false,
                                                    Error = "TRAILER record not match."
                                                };
                                            }
                                            await _context.BulkInsertAsync(stockList);
                                            stopwatch.Stop();
                                            timeElapsed = Math.Ceiling(stopwatch.Elapsed.TotalSeconds);
                                            uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(4, 4), Category = "Stock" } };
                                            await _context.BulkInsertAsync(uploadFileNames);

                                            //for file header
                                            fileStores = stockFileData.Take(1).SelectMany(y => new List<FileStore>
                                            {
                                            new()
                                            {
                                                Header=y.Substring(0,6),
                                                FileName=y.Substring(8,10),
                                                StoreNumber=y.Substring(18,4),
                                                FileDate=exDate,
                                                Category="Stock",
                                            }}).ToList();
                                            await _context.BulkInsertAsync(fileStores);
                                            isSaveSuccess = true;
                                            File.Delete(path);
                                            FileStore fileStore = _context.FileStore.Where(u => u.Category == "Stock" && u.FileDate == tempfileName.Substring(4, 4) &&  u.StoreNumber == stockStoreName && u.Header == "HEADER" && u.FileName == "EXISTENCIA").SingleOrDefault();
                                            fileStore.RecordCount = stockList.Count.ToString();
                                            fileStore.Status = "OKAY";
                                            await _context.SaveChangesAsync();
                                            return new FileUplaodRespone
                                            {
                                                stockRecordCount = stockList.Count.ToString(),
                                                TimeElapsed = timeElapsed,
                                                Success = isSaveSuccess
                                            };
                                        }
                                   // }

                            }
                            catch (Exception e)
                            {
                                File.Delete(path);
                                return new FileUplaodRespone
                                {
                                    Success = false,
                                    Error = "Please select correct file......"
                                };

                            }
                        }
                   // }
                     //   return new FileUplaodRespone
                      //  {
                        //    Success = false,
                        //    Error = "Previous Date not match."
                       // };
                    }
                }
                if (checkFileExtension == zipFileExtension)
                {
                    //for zip file
                    string existingFile = model.File.FileName.ToString();

                    // var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                    // fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.
                    pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");
                    if (!Directory.Exists(pathBuilt))
                    {
                        Directory.CreateDirectory(pathBuilt);
                    }
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files",
                    existingFile);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.File.CopyToAsync(stream);
                    }

                    string zipPath = Path.GetFileName(path);
                    ZipFile.ExtractToDirectory(path, pathBuilt);

                    var filePaths = Directory.GetFiles(pathBuilt, "*.txt");

                    string destinationPath = filePaths[0].ToString();
                    var extension = "." + destinationPath.Split('.')[model.File.FileName.Split('.').Length - 1];
                   
                  //  if (StockDataPreviousDate == exDate || finalStockDate == exDate)
                  //  {

                        if (filetext == reservedFileExtension || stockStoreName == storeNumber || finalStockDate == exDate)
                        {
                        var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == stockStoreName && x.FileDate == finalStockDate && x.Category == "Stock");
                        if (filedata.Result != null)
                        {
                            return new FileUplaodRespone
                            {
                                Success = false,
                                Error = "Stock zip file is already uploaded."
                            };
                        }
                        var zipStockFile = File.ReadAllLines(destinationPath);
                        // get the trailer records 
                        trailerRecords = zipStockFile.TakeLast(1).SelectMany(y => new List<FileStore>
                            {
                            new()
                            {
                            RecordCount=y.Substring(29,6),
                            }}).ToList();
                        string stockInnerFileHeader = zipStockFile[0].Substring(0, 6).Trim();
                        string stockInnerFileName = zipStockFile[0].Substring(7, 11).Trim();
                        string stockInnerStoreNumber = zipStockFile[0].Substring(18, 4).Trim();
                        string stockInnerFileDate = zipStockFile[0].Substring(22, 7).Trim();
                        // string jobOrderInnerDate = string.Format("{0:ddMMyy}", jobOrderInnerFileDate).Trim();
                        try
                        {
                              //  if (stockInnerFileDate == StockDataPreviousInnerDate || stockInnerFileDate == forInnerStockDate)
                              //  {
                                    if (stockInnerFileName == stockFile || stockInnerStoreNumber == storeNumber || stockInnerFileDate == forInnerStockDate)
                                    {
                                        stockList = zipStockFile.Skip(1).SkipLast(1).SelectMany(x => new List<Stock>
                                    {
                                    new()
                                    {
                                       Store=(x.Substring(0,4)),
                                        SKU=Regex.Replace(x.Substring(4,14),regex,""),
                                        // Departament=(x.Substring(18,4)),
                                        Department=Regex.Replace(x.Substring(18,4),regex, ""),
                                        // Description=(x.Substring(22,30)),
                                        Description = Regex.Replace(x.Substring(22, 30), regex, ""),
                                        PrecVtaNorm=Regex.Replace(x.Substring(52,8),regex,""),
                                        PrecVtaNorm_SImpto=Regex.Replace(x.Substring(60,8),regex,""),
                                        SOH=Regex.Replace(x.Substring(68,12),regex,""),
                                        Category=x.Length ==87 ?x.Substring(81,6):null,
                                        CustomerId = model.CustomerId,
                                        StoreId = model.StoreId,
                                        StockDate = currentDate,
                                    }
                                    }).ToList();
                                    }
                               // }
                            // Replaces the matched
                            string str = Regex.Replace(trailerRecords[0].RecordCount, regex, "");
                            if (str == stockList.Count.ToString())
                            { }
                            else
                            {
                                return new FileUplaodRespone
                                {
                                    Success = false,
                                    Error = "TRAILER record not match."
                                };
                            }
                            await _context.BulkInsertAsync(stockList);
                            stopwatch.Stop();
                            timeElapsed = Math.Ceiling(stopwatch.Elapsed.TotalSeconds);

                            uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "Stock" } };
                            await _context.BulkInsertAsync(uploadFileNames);

                            //for file header
                            fileStores = zipStockFile.Take(1).SelectMany(y => new List<FileStore>
                                    {
                                    new()
                                    {
                                            Header=y.Substring(0,6),
                                            FileName=y.Substring(8,10),
                                            StoreNumber=y.Substring(18,4),
                                            FileDate=exDate,
                                    Category="Stock",
                                    }}).ToList();
                            await _context.BulkInsertAsync(fileStores);
                        }
                        catch (Exception e)
                        {

                            File.Delete(pathBuilt);
                            File.Delete(destinationPath);
                            return new FileUplaodRespone
                            {
                                Success = false,
                                Error = e.Message
                            };

                            // Submit the change to the database
                        }
                        }
                   // }
                   
                   isSaveSuccess = true;
                    FileStore fileStore = _context.FileStore.Where(u => u.Category == "Stock"  && u.FileDate== tempfileName.Substring(4, 4) && u.StoreNumber == stockStoreName && u.Header == "HEADER" && u.FileName == "EXISTENCIA").SingleOrDefault();
                    fileStore.RecordCount = stockList.Count.ToString();
                    fileStore.Status = "OKAY";
                    await _context.SaveChangesAsync();

                    return new FileUplaodRespone
                    {
                        stockRecordCount = stockList.Count.ToString(),
                        TimeElapsed = timeElapsed,
                        Success = isSaveSuccess
                    };
                }
            }

            catch (Exception e)
            {
                return new FileUplaodRespone
                {
                    Success = false,
                    Error = e.Message
                };
            }
            return new FileUplaodRespone
            {
                Success = false,
                Error = "Some errors occurred!"
            };
        }
        public async Task<FileUplaodRespone> CatergoriesData(FilterDataModel model)
        {
            TimeZoneInfo localZones = TimeZoneInfo.Local;
            DateTime currentDate = model.StockDate.Value;
            _logger.LogInfo($"Get CatergoriesDate : {currentDate}");
            string regex = "^0+(?!$)";
            bool isSaveSuccess = false;
            string fileName;
            var innerDataError = model.StockDate.Value.Date.AddDays(-1).Day.ToString("#00") + model.StockDate.Value.Date.Month.ToString("#00") +
               model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();

            _logger.LogInfo($"innerDataError : {innerDataError}");

            List<Categories> categoryList = new();
            //for file header
            List<FileStore> fileStores = new();
            List<FileStore> trailerRecords = new();
            //for file name
            List<UploadFileName> uploadFileNames = new();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");

            if (Directory.Exists(pathBuilt))
            {
                DirectoryInfo di = new DirectoryInfo(pathBuilt);
                FileInfo[] files = di.GetFiles();
                foreach (FileInfo file in files)
                {
                    file.Delete();
                }
            }

            double timeElapsed = 0;
            try
            {
                var categoryDate = model.StockDate.ToString();
                var caytegoryStore = model.StoreId.ToString();
                var categoryStoreName = model.StoreName.ToString().Substring(0, 4);

                var finalCategoryDate = model.StockDate.Value.Date.Month.ToString("#00") +
                   model.StockDate.Value.Date.AddDays(-1).Day.ToString("#00"); ;
                //  var forInnerStockDates = Convert.ToDateTime("35-13-2029");
                _logger.LogInfo($"finalCategoryDate : {finalCategoryDate}");
                var categoryPreviousDate = model.StockDate.Value.Date.Month.ToString("#00") +
                  model.StockDate.Value.Date.AddDays(-2).Day.ToString("#00");
                _logger.LogInfo($"categoryPreviousDate : {categoryPreviousDate}");

                var CategoryInnerCategoryDate = model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString()
                         + model.StockDate.Value.Date.Month.ToString("#00")
                         + model.StockDate.Value.Date.AddDays(-1).Day.ToString("#00");
                _logger.LogInfo($"CategoryInnerCategoryDate : {CategoryInnerCategoryDate}");
                var forInnerCategoryDate = string.Empty;
                try
                {
                    forInnerCategoryDate = model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString()
                         + model.StockDate.Value.Date.Month.ToString("#00")
                         + model.StockDate.Value.Date.AddDays(-1).Day.ToString("#00");

                    _logger.LogInfo($"forInnerCategoryDate : {forInnerCategoryDate}");
                }
                catch (Exception ex)
                {
                    return new FileUplaodRespone
                    {
                        Success = false,
                        Error = "Inner Category Date error"
                    };
                }
                var pathFile = model.File.FileName;
                _logger.LogInfo($"Get pathFile : {pathFile}");
                var tempfileName = new DirectoryInfo(pathFile).Name;
                var filetext = tempfileName.Substring(0, 4);
                var exDate = tempfileName.Substring(4, 4);
                _logger.LogInfo($"exDate : {exDate}");
                var storeNumber = tempfileName.Substring(9, 4);
                _logger.LogInfo($"storeNumber : {storeNumber}");
                var checkFileExtension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                _logger.LogInfo($"Get categoryDate : {finalCategoryDate}");

                if (checkFileExtension == txtFileExtension || checkFileExtension == zipFileExtension)
                {
                    _logger.LogInfo($"Get categoryFileExtension : {categoryFileExtension}, categoryStoreName:   {categoryStoreName}, finalCategoryDate:{finalCategoryDate}");
                    _logger.LogInfo($"exDate : {exDate}");
                    //if (finalCategoryDate == exDate|| categoryPreviousDate== exDate)
                    // { 
                    if (filetext != categoryFileExtension || categoryStoreName != storeNumber || finalCategoryDate != exDate)
                        {
                            return new FileUplaodRespone
                            {
                                Success = false,
                                Error = "Please choose correct file."
                            };
                        }
                   // }

                    if (checkFileExtension != zipFileExtension)
                    {
                        // if (finalCategoryDate == exDate || categoryPreviousDate == exDate)
                        // {
                        _logger.LogInfo($"finalCategoryDate : {finalCategoryDate}");
                        _logger.LogInfo($"exDate : {exDate}");
                        if (filetext == categoryFileExtension && categoryStoreName == storeNumber && finalCategoryDate == exDate)
                            {
                            _logger.LogInfo($"Get categoryFileExtension : {categoryFileExtension}, categoryStoreName:   {categoryStoreName}, finalCategoryDate:{finalCategoryDate}, exDate:{exDate} ");
                                var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == categoryStoreName && x.FileDate == finalCategoryDate && x.Category == "Category");
                                if (filedata.Result != null)
                                {
                                    return new FileUplaodRespone
                                    {
                                        Success = false,
                                        // Error = $"Incorrect file with tempfileName : {tempfileName} , filetext:{filetext} , categoryFileExtension:{checkFileExtension} , categoryStoreName:{categoryStoreName} , storeNumber:{storeNumber} , jobOrderDate:{categoryDate} , exDate:{exDate}"
                                        Error = "Incorrect file",// with tempfileName : {tempfileName} , filetext:{filetext} , categoryFileExtension:{checkFileExtension} , categoryStoreName:{categoryStoreName}
                                    };
                                }
                                // condition 

                                var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                                fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.

                                if (!Directory.Exists(pathBuilt))
                                {
                                    Directory.CreateDirectory(pathBuilt);
                                }

                                var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files", fileName);
                                using (var stream = new FileStream(path, FileMode.Create))
                                {
                                    await model.File.CopyToAsync(stream);
                                }
                                var CategoryFileData = File.ReadAllLines(path);
                                //get the trailerRecords
                                trailerRecords = CategoryFileData.TakeLast(1).SelectMany(y => new List<FileStore>
                                        {
                                          new()
                                        {
                                        RecordCount=y.Substring(30,6),
                                        }}).ToList();
                                // for inner header
                                string categoryInnerFileHeader = CategoryFileData[0].Substring(0, 6).Trim();
                                string categoryInnerFileName = CategoryFileData[0].Substring(7, 11).Trim();
                                string categoryInnerStoreNumber = CategoryFileData[0].Substring(18, 4).Trim();
                                string categoryInnerFileDate = CategoryFileData[0].Substring(22, 7).Trim();
                                //string jobOrderInnerDate = "210920";// string.Format("{0:ddMMyy}", jobOrderInnerFileDate).Trim();
                                try
                                {
                                   // if (categoryInnerFileDate == CategoryInnerCategoryDate || forInnerCategoryDate == categoryInnerFileDate)
                                  //  {
                                        if (categoryInnerFileName == categoryFile && categoryInnerStoreNumber == storeNumber)
                                        {
                                            // file content
                                            categoryList = CategoryFileData.Skip(1).SkipLast(1).SelectMany(x => new List<Categories>
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

                                        // Replaces the matched
                                        string str = Regex.Replace(trailerRecords[0].RecordCount, regex, "");
                                        if (str == categoryList.Count.ToString())
                                        { }
                                        else
                                        {
                                            return new FileUplaodRespone
                                            {
                                                Success = false,
                                                Error = "TRAILER record not match."
                                            };
                                        }
                                        await _context.BulkInsertAsync(categoryList);
                                        stopwatch.Stop();
                                        timeElapsed = Math.Ceiling(stopwatch.Elapsed.TotalSeconds);
                                        uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "Category" } };
                                        await _context.BulkInsertAsync(uploadFileNames);

                                        //for file header
                                        fileStores = CategoryFileData.Take(1).SelectMany(y => new List<FileStore>
                                            {
                                            new()
                                            {
                                            Header=y.Substring(0,6),
                                            FileName=y.Substring(8,8),
                                            StoreNumber=y.Substring(18,4),
                                            FileDate=exDate,
                                            Category="Category",
                                            }}).ToList();
                                        await _context.BulkInsertAsync(fileStores);

                                        isSaveSuccess = true;
                                        File.Delete(path);
                                        FileStore fileStore = _context.FileStore.Where(u => u.Category == "Category"  && u.FileDate== tempfileName.Substring(4, 4) && u.StoreNumber == categoryInnerStoreNumber && u.Header == "HEADER" && u.FileName == "CATE DIV").SingleOrDefault();
                                        fileStore.RecordCount = categoryList.Count.ToString();
                                        fileStore.Status = "OKAY";
                                        await _context.SaveChangesAsync();
                                        return new FileUplaodRespone
                                        {
                                            stockRecordCount = categoryList.Count.ToString(),
                                            TimeElapsed = timeElapsed,
                                            Success = isSaveSuccess
                                        };
                                    }
                                //}
                                //    return new FileUplaodRespone
                                  //  {
                                  //      Success = false,
                                   //     Error = "Previous Date not match."
                                   // };
                                }
                                catch (Exception e)
                                {
                                    File.Delete(path);
                                    return new FileUplaodRespone
                                    {
                                        Success = false,
                                        Error = "Please select correct file......"
                                    };

                                }
                            }
                        //}
                         //   return new FileUplaodRespone
                          //  {
                           //     Success = false,
                           //     Error = "Previous Date not match."
                           // };
                            //
                        }
                }
                if (checkFileExtension == zipFileExtension)
                {
                    //for zip file
                    string existingFile = model.File.FileName.ToString();

                    // var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                    // fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.
                    pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");
                    if (!Directory.Exists(pathBuilt))
                    {
                        Directory.CreateDirectory(pathBuilt);
                    }
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files",
                    existingFile);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.File.CopyToAsync(stream);
                    }

                    string zipPath = Path.GetFileName(path);
                    ZipFile.ExtractToDirectory(path, pathBuilt);

                    var filePaths = Directory.GetFiles(pathBuilt, "*.txt");

                    string destinationPath = filePaths[0].ToString();
                    var extension = "." + destinationPath.Split('.')[model.File.FileName.Split('.').Length - 1];
                    // if (categoryPreviousDate == exDate || finalCategoryDate == exDate)
                    // {
                    _logger.LogInfo($"finalCategoryDate : {finalCategoryDate}");
                    _logger.LogInfo($"exDate : {exDate}");
                    if (filetext == categoryFileExtension || categoryStoreName == storeNumber || finalCategoryDate == exDate)
                            {
                                var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == categoryStoreName && x.FileDate == finalCategoryDate && x.Category == "Category" && x.Header == "HEADER" && x.FileName == "CATE DIV");
                                if (filedata.Result != null)
                                {
                                    return new FileUplaodRespone
                                    {
                                        Success = false,
                                        Error = "Category zip file is already uploaded."
                                    };
                                }
                                var categoryFileData = File.ReadAllLines(destinationPath);
                                //get the trailerRecords
                                trailerRecords = categoryFileData.TakeLast(1).SelectMany(y => new List<FileStore>
                                                {
                                                new()
                                                {
                                                     RecordCount=y.Substring(30,6),
                                                }}).ToList();
                                string categoryInnerFileHeader = categoryFileData[0].Substring(0, 6).Trim();
                                string categoryInnerFileName = categoryFileData[0].Substring(7, 11).Trim();
                                string categoryInnerStoreNumber = categoryFileData[0].Substring(18, 4).Trim();
                                string categoryInnerFileDate = categoryFileData[0].Substring(22, 7).Trim();
                                // string jobOrderInnerDate = string.Format("{0:ddMMyy}", jobOrderInnerFileDate).Trim();
                                try
                                {
                            // if (categoryInnerFileDate == CategoryInnerCategoryDate || forInnerCategoryDate == categoryInnerFileDate)
                            //  {
                            _logger.LogInfo($"categoryInnerFileDate : {categoryInnerFileDate}");
                            _logger.LogInfo($"forInnerCategoryDate : {forInnerCategoryDate}");
                            if (categoryInnerFileName == categoryFile || categoryInnerStoreNumber == storeNumber || categoryInnerFileDate == forInnerCategoryDate)
                                    {
                                        categoryList = categoryFileData.Skip(1).SkipLast(1).SelectMany(x => new List<Categories>
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
                                    }
                              //  }
                                    // Replaces the matched
                                    string str = Regex.Replace(trailerRecords[0].RecordCount, regex, "");
                                    if (str == categoryList.Count.ToString())
                                    { }
                                    else
                                    {
                                        return new FileUplaodRespone
                                        {
                                            Success = false,
                                            Error = "TRAILER record not match."
                                        };
                                    }
                                    await _context.BulkInsertAsync(categoryList);
                                    stopwatch.Stop();
                                    timeElapsed = Math.Ceiling(stopwatch.Elapsed.TotalSeconds);

                                    uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "Category" } };
                                    await _context.BulkInsertAsync(uploadFileNames);

                                    //for file header
                                    fileStores = categoryFileData.Take(1).SelectMany(y => new List<FileStore>
                                            {
                                            new()
                                            {
                                                        Header=y.Substring(0,6),
                                                    FileName=y.Substring(8,8),
                                                    StoreNumber=y.Substring(18,4),
                                                    FileDate=exDate,
                                            Category="Category",
                                            }}).ToList();
                                    await _context.BulkInsertAsync(fileStores);

                                }
                                catch (Exception e)
                                {

                                    File.Delete(pathBuilt);
                                    File.Delete(destinationPath);
                                    return new FileUplaodRespone
                                    {
                                        Success = false,
                                        Error = e.Message
                                    };

                                    // Submit the change to the database
                                }
                            }
                   // }

                    isSaveSuccess = true;
                    FileStore fileStore = _context.FileStore.Where(u => u.Category == "Category" && u.FileDate== tempfileName.Substring(4, 4) && u.StoreNumber == model.StoreName.ToString() && u.Header == "HEADER" && u.FileName == "CATE DIV").SingleOrDefault();
                    fileStore.RecordCount = categoryList.Count.ToString();
                    fileStore.Status = "OKAY";
                    await _context.SaveChangesAsync();

                    return new FileUplaodRespone
                    {
                        stockRecordCount = categoryList.Count.ToString(),
                        TimeElapsed = timeElapsed,
                        Success = isSaveSuccess
                    };
                }
            }
            catch (Exception e)
            {
                return new FileUplaodRespone
                {
                    Success = false,
                    Error = e.Message
                };
            }
            return new FileUplaodRespone
            {
                Success = false,
                Error = "Some errors occurred!"
            };
        }
    }
}
