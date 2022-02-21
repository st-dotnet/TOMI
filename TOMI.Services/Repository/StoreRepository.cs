using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
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
            var innerDataError = model.StockDate.Value.Date.Day.ToString("#00") + model.StockDate.Value.Date.Month.ToString("#00") +
                model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
            List<OrderJob> orderJobList = new();
            //for file header
            List<FileStore> fileStores = new();
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
                var jobDate = model.StockDate.ToString();
                var jobOrderStore = model.StoreId.ToString();
                var jobOrderStoreName = model.StoreName.ToString();

                var jobOrderDate = model.StockDate.Value.Date.Month.ToString("#00") +
                   model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
                //  var forInnerStockDates = Convert.ToDateTime("35-13-2029");
                var forInnerStockDate = string.Empty;
                try
                {
                    forInnerStockDate = model.StockDate.Value.Date.AddDays(-1).Day.ToString("#00") + model.StockDate.Value.Date.Month.ToString("#00") +

               model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
                }
                catch (Exception ex)
                {
                    return new FileUplaodRespone
                    {
                        Success = false,
                        Error = "Inner StockDate error"
                    };
                }

                //  var stockDate = model.StockDate.Value.Date;

                // var forInnerStockDate =  stockDate.Day.ToString("#00")
                // + stockDate.Month.ToString("#00")
                //    + stockDate.Year.ToString().Substring(2, 2).ToString();

                var tempfileName = model.File.FileName;
                var filetext = tempfileName.Substring(0, 4);
                var exDate = tempfileName.Substring(4, 4);
                var storeNumber = tempfileName.Substring(9, 4);
                var checkFileExtension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];

                if (checkFileExtension == txtFileExtension || checkFileExtension == zipFileExtension)
                {
                    if (filetext != masterFileExtension || jobOrderStoreName != storeNumber || jobOrderDate != exDate)
                    {
                        return new FileUplaodRespone
                        {
                            Success = false,
                            Error = "Please choose correct file."
                        };
                    }

                    if (checkFileExtension != zipFileExtension)
                    {
                        if (filetext == masterFileExtension && jobOrderStoreName == storeNumber && jobOrderDate == exDate)
                        {
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
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await model.File.CopyToAsync(stream);
                            }


                            string regex = "^0+(?!$)";
                            var orderJobFile = File.ReadAllLines(path);
                            // for inner header
                            string jobOrderInnerFileHeader = orderJobFile[0].Substring(0, 6).Trim();
                            string jobOrderInnerFileName = orderJobFile[0].Substring(7, 11).Trim();
                            string jobOrderInnerStoreNumber = orderJobFile[0].Substring(18, 4).Trim();
                            string jobOrderInnerFileDate = orderJobFile[0].Substring(22, 7).Trim();
                            //string jobOrderInnerDate = "210920";// string.Format("{0:ddMMyy}", jobOrderInnerFileDate).Trim();
                            try
                            {
                                if (jobOrderInnerFileName == masterFile && jobOrderInnerStoreNumber == storeNumber && jobOrderInnerFileDate == forInnerStockDate)
                                {
                                    // file content
                                    orderJobList = orderJobFile.Skip(1).SelectMany(x => new List<OrderJob>
                                    {
                                    new()
                                    {
                                        //Store = Regex.Replace(x.Substring(0,3),regex, ""),
                                        //Code= Regex.Replace(x.Substring(3,16),regex, ""),
                                        //Department=Regex.Replace(x.Substring(20,4),regex, ""),
                                        //Description =Regex.Replace(x.Substring(24,30),regex, ""),
                                        //SalePrice= Regex.Replace(x.Substring(54,8),regex, ""),
                                        //PriceWithoutTaxes= Regex.Replace(x.Substring(60,8),regex, ""),
                                        //SKU = Regex.Replace(x.Substring(70, 14), regex, ""),
                                        //Category=x.Length ==88 ?x.Substring(82,6):null,
                                         Store = Regex.Replace(x.Substring(0,3),regex, ""),
                                        Code= Regex.Replace(x.Substring(3,15),regex, ""),
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

                                    // Submit the change to the database.
                                    try
                                    {
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
                                        FileName=y.Substring(7,7),
                                        StoreNumber=y.Substring(18,4),
                                        FileDate=exDate,
                                        Category="OrderJob",
                                    }}).ToList();
                                        await _context.BulkInsertAsync(fileStores);
                                    }
                                    catch (Exception e)
                                    {
                                        File.Delete(path);
                                        return new FileUplaodRespone
                                        {
                                            Success = false,
                                            Error = e.Message
                                        };

                                    }
                                    isSaveSuccess = true;
                                    File.Delete(path);
                                    FileStore fileStore = _context.FileStore.Where(u => u.Category == "OrderJob" && u.StoreNumber == model.StoreName.ToString() && u.FileDate == jobOrderDate).SingleOrDefault();
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
                                File.Delete(path);
                                return new FileUplaodRespone
                                {
                                    Success = false,
                                    Error = "Please select correct file......"
                                };

                            }
                        }
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

                    if (filetext == masterFileExtension || jobOrderStoreName == storeNumber || jobOrderDate == exDate)
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
                        string regex = "^0+(?!$)";

                        string jobOrderInnerFileHeader = zipOrderJobFile[0].Substring(0, 6).Trim();
                        string jobOrderInnerFileName = zipOrderJobFile[0].Substring(7, 11).Trim();
                        string jobOrderInnerStoreNumber = zipOrderJobFile[0].Substring(18, 4).Trim();
                        string jobOrderInnerFileDate = zipOrderJobFile[0].Substring(22, 7).Trim();
                        // string jobOrderInnerDate = string.Format("{0:ddMMyy}", jobOrderInnerFileDate).Trim();
                        try
                        {
                            if (jobOrderInnerFileName == masterFile || jobOrderInnerStoreNumber == storeNumber || jobOrderInnerFileDate == forInnerStockDate)
                            {
                                orderJobList = zipOrderJobFile.Skip(1).SelectMany(x => new List<OrderJob>
                                    {
                                    new()
                                    {
                                      Store = Regex.Replace(x.Substring(0,3),regex, ""),
                                        Code= Regex.Replace(x.Substring(3,15),regex, ""),
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
                                    FileName=y.Substring(7,11),
                                    StoreNumber=y.Substring(18,4),
                                    FileDate=string.Format("{0:MMyy}",y.Substring(24,4)),
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
                    isSaveSuccess = true;
                    FileStore fileStore = _context.FileStore.Where(u => u.Category == "OrderJob" && u.StoreNumber == model.StoreName.ToString() && u.FileDate == jobOrderDate).SingleOrDefault();
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
                Error = "Some errors occurred!" + innerDataError
            };
        }
        public async Task<FileUplaodRespone> DepartmentsData(FilterDataModel model)
        {
            bool isSaveSuccess = false;
            string fileName;
            var innerDataError = model.StockDate.Value.Date.Day.ToString("#00") + model.StockDate.Value.Date.Month.ToString("#00") +
                model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
            List<Departments> departmentsList = new();
            //for file header
            List<FileStore> fileDepartment = new();
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

                var deptartmentDate = model.StockDate.ToString();
                var deptartmentStore = model.StoreId.ToString();
                var deptartmentStoreName = model.StoreName.ToString();

                var finaldeptartmentDate = model.StockDate.Value.Date.Month.ToString("#00") +
                   model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
                //  var forInnerStockDates = Convert.ToDateTime("35-13-2029");
                var forInnerdeptartmentDate = string.Empty;
                try
                {
                    forInnerdeptartmentDate = model.StockDate.Value.Date.AddDays(-1).Day.ToString("#00") + model.StockDate.Value.Date.Month.ToString("#00") +
                    model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
                }
                catch (Exception ex)
                {
                    return new FileUplaodRespone
                    {
                        Success = false,
                        Error = "Inner DepartmentDate error"
                    };
                }

                //  var stockDate = model.StockDate.Value.Date;

                // var forInnerStockDate =  stockDate.Day.ToString("#00")
                // + stockDate.Month.ToString("#00")
                //    + stockDate.Year.ToString().Substring(2, 2).ToString();

                var tempfileName = model.File.FileName;
                var filetext = tempfileName.Substring(0, 4);
                var exDate = tempfileName.Substring(4, 4);
                var storeNumber = tempfileName.Substring(9, 4);
                var checkFileExtension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];



                if (checkFileExtension == txtFileExtension || checkFileExtension == zipFileExtension)
                {
                    if (filetext != deptFileExtension && deptartmentStoreName != storeNumber && finaldeptartmentDate != exDate)
                    {
                        return new FileUplaodRespone
                        {
                            Success = false,
                            Error = "Please choose correct file."
                        };
                    }

                    if (checkFileExtension != zipFileExtension)
                    {
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
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await model.File.CopyToAsync(stream);
                            }

                            var deptFileData = File.ReadAllLines(path);

                            string regex = "^0+(?!$)";
                            string deptInnerFileHeader = deptFileData[0].Substring(0, 6).Trim();
                            string deptInnerFileName = deptFileData[0].Substring(7, 5).Trim();
                            string deptInnerStoreNumber = deptFileData[0].Substring(18, 4).Trim();
                            string deptInnerFileDate = deptFileData[0].Substring(22, 7).Trim();
                            try
                            {
                                if (deptInnerFileName == deptFileExtension && deptInnerStoreNumber == storeNumber && deptInnerFileDate == forInnerdeptartmentDate)
                                {
                                    departmentsList = deptFileData.Skip(1).SelectMany(x => new List<Departments>
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
                                try
                                {
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
                                        FileName=y.Substring(7,7),
                                        StoreNumber=y.Substring(18,4),
                                        FileDate=exDate,
                                        Category="DPTO",
                                    }}).ToList();
                                    await _context.BulkInsertAsync(fileDepartment);
                                }
                                catch (Exception e)
                                {
                                    File.Delete(path);
                                    return new FileUplaodRespone
                                    {
                                        Success = false,
                                        Error = e.Message
                                    };
                                }
                                isSaveSuccess = true;
                                File.Delete(path);
                                FileStore fileStore = _context.FileStore.Where(u => u.Category == "DPTO" && u.StoreNumber == model.StoreName.ToString() && u.FileDate == finaldeptartmentDate).SingleOrDefault();
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
                        string regex = "^0+(?!$)";
                        string deptInnerFileHeader = zipDepartmentFile[0].Substring(0, 6).Trim();
                        string deptInnerFileName = zipDepartmentFile[0].Substring(7, 5).Trim();
                        string deptInnerStoreNumber = zipDepartmentFile[0].Substring(18, 4).Trim();
                        string deptInnerFileDate = zipDepartmentFile[0].Substring(22, 7).Trim();
                        // string stockInnerDate = string.Format("{0:ddMMyy}", stockInnerFileDate).Trim();
                        try
                        {
                            if (deptInnerFileName == deptFileExtension || deptInnerStoreNumber == storeNumber || deptInnerFileDate == finaldeptartmentDate)
                            {
                                departmentsList = zipDepartmentFile.Skip(1).SelectMany(x => new List<Departments>
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
                            await _context.BulkInsertAsync(departmentsList);
                            stopwatch.Stop();
                            timeElapsed = Math.Ceiling(stopwatch.Elapsed.TotalSeconds);
                            uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "DPTO" } };
                            await _context.BulkInsertAsync(uploadFileNames);

                            fileDepartment = zipDepartmentFile.Take(1).SelectMany(y => new List<FileStore>
                            {
                            new()
                            {
                            Header=(y.Substring(0,6)),
                            FileName=(y.Substring(7,11)),
                            StoreNumber=(y.Substring(18,4)),
                            FileDate=string.Format("{0:MMyy}",y.Substring(24,4)),
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
                    isSaveSuccess = true;
                    FileStore fileStore = _context.FileStore.Where(u => u.Category == "DPTO" && u.StoreNumber == model.StoreName.ToString() && u.FileDate == finaldeptartmentDate).SingleOrDefault();
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
                Error = "Some errors occurred!" + innerDataError
            };
        }
        public async Task<FileUplaodRespone> ReservedData(FilterDataModel model)
        {
            bool isSaveSuccess = false;
            string fileName;
            var innerDataError = model.StockDate.Value.Date.Day.ToString("#00") + model.StockDate.Value.Date.Month.ToString("#00") +
                model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
            List<Reserved> reservedList = new();
            //for file header
            List<FileStore> fileStores = new();
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
                var reservedStoreName = model.StoreName.ToString();

                var finalReservedDate = model.StockDate.Value.Date.Month.ToString("#00") +
                   model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
                //  var forInnerStockDates = Convert.ToDateTime("35-13-2029");
                var finalInnerreservedDate = string.Empty;
                try
                {
                    finalInnerreservedDate = model.StockDate.Value.Date.AddDays(-1).Day.ToString("#00") + model.StockDate.Value.Date.Month.ToString("#00") +
                    model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
                }
                catch (Exception ex)
                {
                    return new FileUplaodRespone
                    {
                        Success = false,
                        Error = "Inner Reserved Date error"
                    };
                }
                var tempfileName = model.File.FileName;
                var filetext = tempfileName.Substring(0, 4);
                var exDate = tempfileName.Substring(4, 4);
                var storeNumber = tempfileName.Substring(9, 4);
                var checkFileExtension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];

                if (checkFileExtension == txtFileExtension || checkFileExtension == zipFileExtension)
                {
                    if (filetext != reservedFileExtension || reservedStoreName != storeNumber || finalReservedDate != exDate)
                    {
                        return new FileUplaodRespone
                        {
                            Success = false,
                            Error = "Please choose correct file."
                        };
                    }

                    if (checkFileExtension != zipFileExtension)
                    {
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


                            string regex = "^0+(?!$)";
                            var ReservedFile = File.ReadAllLines(path);
                            // for inner header
                            string reservedInnerFileHeader = ReservedFile[0].Substring(0, 6).Trim();
                            string reservedInnerFileName = ReservedFile[0].Substring(7, 5).Trim();
                            string reservedInnerStoreNumber = ReservedFile[0].Substring(18, 4).Trim();
                            string reservedInnerFileDate = ReservedFile[0].Substring(22, 7).Trim();
                            //string jobOrderInnerDate = "210920";// string.Format("{0:ddMMyy}", jobOrderInnerFileDate).Trim();
                            try
                            {
                                if (reservedInnerFileName == reservedFileExtension && reservedInnerStoreNumber == storeNumber && reservedInnerFileDate == finalInnerreservedDate)
                                {
                                    // file content
                                    reservedList = ReservedFile.Skip(1).SelectMany(x => new List<Reserved>
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
                                        uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "Reserved" } };
                                        await _context.BulkInsertAsync(uploadFileNames);

                                        //for file header
                                        fileStores = ReservedFile.Take(1).SelectMany(y => new List<FileStore>
                                        {
                                        new()
                                        {
                                            Header=(y.Substring(0,6)),
                                            FileName=(y.Substring(7,11)),
                                            StoreNumber=(y.Substring(18,4)),
                                            FileDate=string.Format("{0:MMyy}",y.Substring(24,4)),
                                            Category="Reserved",
                                            }}).ToList();
                                        await _context.BulkInsertAsync(fileStores);
                                    }
                                    catch (Exception e)
                                    {
                                        File.Delete(path);
                                        return new FileUplaodRespone
                                        {
                                            Success = false,
                                            Error = e.Message
                                        };

                                    }
                                    isSaveSuccess = true;
                                    File.Delete(path);
                                    FileStore fileStore = _context.FileStore.Where(u => u.Category == "Reserved" && u.StoreNumber == model.StoreName.ToString() && u.FileDate == finalReservedDate).SingleOrDefault();
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
                                File.Delete(path);
                                return new FileUplaodRespone
                                {
                                    Success = false,
                                    Error = "Please select correct file......"
                                };

                            }
                        }
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
                        string regex = "^0+(?!$)";

                        string jobOrderInnerFileHeader = zipReservedFile[0].Substring(0, 6).Trim();
                        string jobOrderInnerFileName = zipReservedFile[0].Substring(7, 11).Trim();
                        string jobOrderInnerStoreNumber = zipReservedFile[0].Substring(18, 4).Trim();
                        string jobOrderInnerFileDate = zipReservedFile[0].Substring(22, 7).Trim();
                        // string jobOrderInnerDate = string.Format("{0:ddMMyy}", jobOrderInnerFileDate).Trim();
                        try
                        {
                            if (jobOrderInnerFileName == reservedFile || jobOrderInnerStoreNumber == storeNumber || jobOrderInnerFileDate == finalInnerreservedDate)
                            {
                                reservedList = zipReservedFile.Skip(1).SelectMany(x => new List<Reserved>
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
                                    FileName=y.Substring(7,11),
                                    StoreNumber=y.Substring(18,4),
                                    FileDate=string.Format("{0:MMyy}",y.Substring(24,4)),
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
                    isSaveSuccess = true;
                    FileStore fileStore = _context.FileStore.Where(u => u.Category == "Reserved" && u.StoreNumber == model.StoreName.ToString() && u.FileDate ==finalReservedDate).SingleOrDefault();
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
                Error = "Some errors occurred!" + innerDataError
            };

        }
        public async Task<FileUplaodRespone> StockData(FilterDataModel model)
        {
            bool isSaveSuccess = false;
            string fileName;
            var innerDataError = model.StockDate.Value.Date.Day.ToString("#00") + model.StockDate.Value.Date.Month.ToString("#00") +
                model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
            List<Stock> stockList = new();
            //for file header
            List<FileStore> fileStores = new();
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
                var stockDate = model.StockDate.ToString();
                var stockStore = model.StoreId.ToString();
                var stockStoreName = model.StoreName.ToString();

                var finalStockDate = model.StockDate.Value.Date.Month.ToString("#00") +
                   model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
                //  var forInnerStockDates = Convert.ToDateTime("35-13-2029");
                var forInnerStockDate = string.Empty;
                try
                {
                    forInnerStockDate = model.StockDate.Value.Date.AddDays(-1).Day.ToString("#00") + model.StockDate.Value.Date.Month.ToString("#00") +

               model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
                }
                catch (Exception ex)
                {
                    return new FileUplaodRespone
                    {
                        Success = false,
                        Error = "Inner Stock Date error"
                    };
                }

                var tempfileName = model.File.FileName;
                var filetext = tempfileName.Substring(0, 4);
                var exDate = tempfileName.Substring(4, 4);
                var storeNumber = tempfileName.Substring(9, 4);
                var checkFileExtension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];

                if (checkFileExtension == txtFileExtension || checkFileExtension == zipFileExtension)
                {
                    if (filetext != stockFileExtension || stockStoreName != storeNumber || finalStockDate != exDate)
                    {
                        return new FileUplaodRespone
                        {
                            Success = false,
                            Error = "Please choose correct file."
                        };
                    }

                    if (checkFileExtension != zipFileExtension)
                    {
                        if (filetext == stockFileExtension && stockStoreName == storeNumber && finalStockDate == exDate)
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


                            string regex = "^0+(?!$)";
                            var stockFileData = File.ReadAllLines(path);
                            // for inner header
                            string stockInnerFileHeader = stockFileData[0].Substring(0, 6).Trim();
                            string stockInnerFileName = stockFileData[0].Substring(7, 11).Trim();
                            string stockInnerStoreNumber = stockFileData[0].Substring(18, 4).Trim();
                            string stockInnerFileDate = stockFileData[0].Substring(22, 7).Trim();
                            //string jobOrderInnerDate = "210920";// string.Format("{0:ddMMyy}", jobOrderInnerFileDate).Trim();
                            try
                            {
                                if (stockInnerFileName == stockFile && stockInnerStoreNumber == storeNumber && stockInnerFileDate == forInnerStockDate)
                                {
                                    // file content
                                    stockList = stockFileData.Skip(1).SelectMany(x => new List<Stock>
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
                                        uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "Stock" } };
                                        await _context.BulkInsertAsync(uploadFileNames);

                                        //for file header
                                        fileStores = stockFileData.Take(1).SelectMany(y => new List<FileStore>
                                            {
                                            new()
                                            {
                                                 Header=(y.Substring(0,6)),
                                                FileName=(y.Substring(7,11)),
                                                StoreNumber=(y.Substring(18,4)),
                                                FileDate=string.Format("{0:MMyy}",y.Substring(24,4)),
                                                Category="Stock",
                                            }}).ToList();
                                        await _context.BulkInsertAsync(fileStores);
                                    }
                                    catch (Exception e)
                                    {
                                        File.Delete(path);
                                        return new FileUplaodRespone
                                        {
                                            Success = false,
                                            Error = e.Message
                                        };

                                    }
                                    isSaveSuccess = true;
                                    File.Delete(path);
                                    FileStore fileStore = _context.FileStore.Where(u => u.Category == "Stock" && u.StoreNumber == model.StoreName.ToString() && u.FileDate == finalStockDate).SingleOrDefault();
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
                                File.Delete(path);
                                return new FileUplaodRespone
                                {
                                    Success = false,
                                    Error = "Please select correct file......"
                                };

                            }
                        }
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

                    if (filetext == reservedFileExtension || stockStoreName == storeNumber || stockDate == exDate)
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
                        string regex = "^0+(?!$)";

                        string stockInnerFileHeader = zipStockFile[0].Substring(0, 6).Trim();
                        string stockInnerFileName = zipStockFile[0].Substring(7, 11).Trim();
                        string stockInnerStoreNumber = zipStockFile[0].Substring(18, 4).Trim();
                        string stockInnerFileDate = zipStockFile[0].Substring(22, 7).Trim();
                        // string jobOrderInnerDate = string.Format("{0:ddMMyy}", jobOrderInnerFileDate).Trim();
                        try
                        {
                            if (stockInnerFileName == stockFile || stockInnerStoreNumber == storeNumber || stockInnerFileDate == forInnerStockDate)
                            {
                                stockList = zipStockFile.Skip(1).SelectMany(x => new List<Stock>
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
                                    Header=(y.Substring(0,6)),
                                    FileName=(y.Substring(7,11)),
                                    StoreNumber=(y.Substring(18,4)),
                                    FileDate=string.Format("{0:MMyy}",y.Substring(24,4)),
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
                    isSaveSuccess = true;
                    FileStore fileStore = _context.FileStore.Where(u => u.Category == "Stock" && u.StoreNumber == model.StoreName.ToString() && u.FileDate == finalStockDate).SingleOrDefault();
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
                Error = "Some errors occurred!" + innerDataError
            };
        }
        public async Task<FileUplaodRespone> CatergoriesData(FilterDataModel model)
        {

            bool isSaveSuccess = false;
            string fileName;
            var innerDataError = model.StockDate.Value.Date.Day.ToString("#00") + model.StockDate.Value.Date.Month.ToString("#00") +
                model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
            List<Categories> categoryList = new();
            //for file header
            List<FileStore> fileStores = new();
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
                var categoryStoreName = model.StoreName.ToString();

                var finalCategoryDate = model.StockDate.Value.Date.Month.ToString("#00") +
                   model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
                //  var forInnerStockDates = Convert.ToDateTime("35-13-2029");
                var forInnerCategoryDate = string.Empty;
                try
                {
                    forInnerCategoryDate = model.StockDate.Value.Date.AddDays(-1).Day.ToString("#00") + model.StockDate.Value.Date.Month.ToString("#00") +

               model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
                }
                catch (Exception ex)
                {
                    return new FileUplaodRespone
                    {
                        Success = false,
                        Error = "Inner Category Date error"
                    };
                }

                //  var stockDate = model.StockDate.Value.Date;

                // var forInnerStockDate =  stockDate.Day.ToString("#00")
                // + stockDate.Month.ToString("#00")
                //    + stockDate.Year.ToString().Substring(2, 2).ToString();

                var tempfileName = model.File.FileName;
                var filetext = tempfileName.Substring(0, 4);
                var exDate = tempfileName.Substring(4, 4);
                var storeNumber = tempfileName.Substring(9, 4);
                var checkFileExtension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];

                if (checkFileExtension == txtFileExtension || checkFileExtension == zipFileExtension)
                {
                    if (filetext != categoryFileExtension || categoryStoreName != storeNumber || finalCategoryDate != exDate)
                    {
                        return new FileUplaodRespone
                        {
                            Success = false,
                            Error = "Please choose correct file."
                        };
                    }

                    if (checkFileExtension != zipFileExtension)
                    {
                        if (filetext == categoryFileExtension && categoryStoreName == storeNumber && finalCategoryDate == exDate)
                        {
                            var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == categoryStoreName && x.FileDate == finalCategoryDate && x.Category == "Category");
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


                            string regex = "^0+(?!$)";
                            var CategoryFileData = File.ReadAllLines(path);
                            // for inner header
                            string categoryInnerFileHeader = CategoryFileData[0].Substring(0, 6).Trim();
                            string categoryInnerFileName = CategoryFileData[0].Substring(7, 11).Trim();
                            string categoryInnerStoreNumber = CategoryFileData[0].Substring(18, 4).Trim();
                            string categoryInnerFileDate = CategoryFileData[0].Substring(22, 7).Trim();
                            //string jobOrderInnerDate = "210920";// string.Format("{0:ddMMyy}", jobOrderInnerFileDate).Trim();
                            try
                            {
                                if (categoryInnerFileName == categoryFile && categoryInnerStoreNumber == storeNumber && categoryInnerFileDate == forInnerCategoryDate)
                                {
                                    // file content
                                    categoryList = CategoryFileData.Skip(1).SelectMany(x => new List<Categories>
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
                                        FileName=y.Substring(7,7),
                                        StoreNumber=y.Substring(18,4),
                                        FileDate=exDate,
                                        Category="Category",
                                    }}).ToList();
                                        await _context.BulkInsertAsync(fileStores);
                                    }
                                    catch (Exception e)
                                    {
                                        File.Delete(path);
                                        return new FileUplaodRespone
                                        {
                                            Success = false,
                                            Error = e.Message
                                        };

                                    }
                                    isSaveSuccess = true;
                                    File.Delete(path);
                                    FileStore fileStore = _context.FileStore.Where(u => u.Category == "Category" && u.StoreNumber == model.StoreName.ToString() && u.FileDate == finalCategoryDate).SingleOrDefault();
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
                                File.Delete(path);
                                return new FileUplaodRespone
                                {
                                    Success = false,
                                    Error = "Please select correct file......"
                                };

                            }
                        }
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

                    if (filetext == categoryFileExtension || categoryStoreName == storeNumber || categoryDate == exDate)
                    {
                        var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == categoryStoreName && x.FileDate == finalCategoryDate && x.Category == "Category");
                        if (filedata.Result != null)
                        {
                            return new FileUplaodRespone
                            {
                                Success = false,
                                Error = "Category zip file is already uploaded."
                            };
                        }
                        var categoryFileData = File.ReadAllLines(destinationPath);
                        string regex = "^0+(?!$)";

                        string categoryInnerFileHeader = categoryFileData[0].Substring(0, 6).Trim();
                        string categoryInnerFileName = categoryFileData[0].Substring(7, 11).Trim();
                        string categoryInnerStoreNumber = categoryFileData[0].Substring(18, 4).Trim();
                        string categoryInnerFileDate = categoryFileData[0].Substring(22, 7).Trim();
                        // string jobOrderInnerDate = string.Format("{0:ddMMyy}", jobOrderInnerFileDate).Trim();
                        try
                        {
                            if (categoryInnerFileName == categoryFile || categoryInnerStoreNumber == storeNumber || categoryInnerFileDate == forInnerCategoryDate)
                            {
                                categoryList = categoryFileData.Skip(1).SelectMany(x => new List<Categories>
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
                                    FileName=y.Substring(7,11),
                                    StoreNumber=y.Substring(18,4),
                                    FileDate=string.Format("{0:MMyy}",y.Substring(24,4)),
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
                    isSaveSuccess = true;
                    FileStore fileStore = _context.FileStore.Where(u => u.Category == "Category" && u.StoreNumber == model.StoreName.ToString() && u.FileDate == finalCategoryDate).SingleOrDefault();
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
                Error = "Some errors occurred!" + innerDataError
            };

        }
    }
}
