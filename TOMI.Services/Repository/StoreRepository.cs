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
            //for file header
            List<FileStore> fileStores = new();
            //for file name
            List<UploadFileName> uploadFileNames = new();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            double timeElapsed = 0;
            try
            {
                string jobDate = model.StockDate.ToString();
                string jobOrderStore = model.StoreId.ToString();
                string jobOrderStoreName = model.StoreName.ToString();
                string jobOrderDate = String.Format("{0:MMyy}", model.StockDate);
                string forInnerStockDate = String.Format("{0:ddMMyy}", model.StockDate);
                var tempfileName = model.File.FileName;
                string filetext = tempfileName.Substring(0, 4);
                string fileDate = tempfileName.Substring(4, 4);
                string exDate = string.Format("{0:MMyy}", fileDate);
                string storeNumber = tempfileName.Substring(9, 4);
                var checkFileExtension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];

                if (checkFileExtension != ".txt" || checkFileExtension != ".zip")
                {
                    if (filetext != "DPTO" && jobOrderStoreName != storeNumber && jobOrderDate != exDate)
                    {
                        return new FileUplaodRespone
                        {
                            Success = false,
                            Error = "please choose correct file."
                        };
                    }

                    if (checkFileExtension == ".txt")
                    {
                        if (filetext == "DPTO" && jobOrderStoreName == storeNumber && jobOrderDate == exDate)
                        {
                            var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == jobOrderStoreName && x.FileDate == jobOrderDate && x.Category== "DPTO");
                            if (filedata.Result != null)
                            {
                                return new FileUplaodRespone
                                {
                                    Success = false,
                                    Error = "file is already uploaded."
                                };
                            }
                            var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                            fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.
                            var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");
                            if (!Directory.Exists(pathBuilt))
                            {
                                Directory.CreateDirectory(pathBuilt);
                            }
                            var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await model.File.CopyToAsync(stream);
                            }
                            var deptFile = File.ReadAllLines(path);
                            string stockInnerFileHeader = deptFile[0].Substring(0, 6).Trim();
                            string stockInnerFileName = deptFile[0].Substring(7, 11).Trim();
                            string stockInnerStoreNumber = deptFile[0].Substring(18, 4).Trim();
                            string stockInnerFileDate = deptFile[0].Substring(22, 7).Trim();
                            string stockInnerDate = string.Format("{0:ddMMyy}", stockInnerFileDate).Trim();
                            if (stockInnerFileName == "DPTO DIV" && stockInnerStoreNumber == storeNumber && stockInnerDate == forInnerStockDate)
                            {
                                uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "DPTO" } };
                                await _context.BulkInsertAsync(uploadFileNames);
                                //for file header
                                fileStores = deptFile.Take(1).SelectMany(y => new List<FileStore>
                                {
                                new()
                                {
                                Header=(y.Substring(0,6)),
                                FileName=(y.Substring(7,11)),
                                StoreNumber=(y.Substring(18,4)),
                                FileDate=string.Format("{0:MMyy}",y.Substring(24,4)),
                                Category="DPTO",
                                }}).ToList();
                                await _context.BulkInsertAsync(fileStores);
                                string regex = "^0+(?!$)";
                                // file content
                                departmentsList = deptFile.Skip(1).SelectMany(x => new List<Departments>
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
                                    FileStore fileStore = _context.FileStore.Where(u => u.Category == "DPTO" && u.StoreNumber == model.StoreName.ToString() && u.FileDate == exDate).SingleOrDefault();
                                    fileStore.RecordCount = departmentsList.Count.ToString();
                                    await _context.SaveChangesAsync();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }
                                isSaveSuccess = true;
                            }
                        }
                    }
                }
                if (checkFileExtension == ".zip")
                {
                    //for zip file
                    string existingFile = model.File.FileName.ToString();

                    // var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                    // fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.
                    var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");
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
                    if (filetext == "DPTO" && jobOrderStoreName == storeNumber && jobOrderDate == exDate)
                    {
                        var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == jobOrderStoreName && x.FileDate == jobOrderDate && x.Category== "DPTO");
                        if (filedata.Result != null)
                        {
                            return new FileUplaodRespone
                            {
                                Success = false,
                                Error = "file is already uploaded."
                            };
                        }
                        var deptFile = File.ReadAllLines(destinationPath);
                        string regex = "^0+(?!$)";
                        string stockInnerFileHeader = deptFile[0].Substring(0, 6).Trim();
                        string stockInnerFileName = deptFile[0].Substring(7, 11).Trim();
                        string stockInnerStoreNumber = deptFile[0].Substring(18, 4).Trim();
                        string stockInnerFileDate = deptFile[0].Substring(22, 7).Trim();
                        string stockInnerDate = string.Format("{0:ddMMyy}", stockInnerFileDate).Trim();

                        if (stockInnerFileName == "DPTO DIV" && stockInnerStoreNumber == storeNumber && stockInnerDate == forInnerStockDate)
                        {
                            uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "DPTO" } };
                            await _context.BulkInsertAsync(uploadFileNames);

                            //for file header
                            fileStores = deptFile.Take(1).SelectMany(y => new List<FileStore>
                            {
                            new()
                            {
                            Header=(y.Substring(0,6)),
                            FileName=(y.Substring(7,11)),
                            StoreNumber=(y.Substring(18,4)),
                            FileDate=string.Format("{0:MMyy}",y.Substring(24,4)),
                            Category="DPTO",
                            }}).ToList();
                            await _context.BulkInsertAsync(fileStores);
                            }
                            departmentsList = deptFile.Skip(1).SelectMany(x => new List<Departments>
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
                            // File.Delete(pathBuilt);
                            File.Delete(destinationPath);
                            
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    isSaveSuccess = true;
                    FileStore fileStore = _context.FileStore.Where(u => u.Category == "DPTO" && u.StoreNumber == model.StoreName.ToString() && u.FileDate == exDate).SingleOrDefault();
                    fileStore.RecordCount = departmentsList.Count.ToString();
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return new FileUplaodRespone
            {
                stockRecordCount = departmentsList.Count.ToString(),
                TimeElapsed = timeElapsed,
                Success = isSaveSuccess
            };

        }
        public async Task<FileUplaodRespone> StockData(FilterDataModel model)
        {
            bool isSaveSuccess = false;
            string fileName;
            List<Stock> stockList = new();
            //for file header
            List<FileStore> fileStores = new();
            //for file name
            List<UploadFileName> uploadFileNames = new();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            double timeElapsed = 0;
            try
            {
                string StockDate = model.StockDate.ToString();
                string StockStore = model.StoreId.ToString();
                string StockStoreName = model.StoreName.ToString();
                string ActStockDate = String.Format("{0:MMyy}", model.StockDate);
                string forInnerStockDate = String.Format("{0:ddMMyy}", model.StockDate);

                var tempfileName = model.File.FileName;
                string filetext = tempfileName.Substring(0, 4);
                string fileDate = tempfileName.Substring(4, 4);
                string exDate = string.Format("{0:MMyy}", fileDate);
                string storeNumber = tempfileName.Substring(9, 4);
                var checkFileExtension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];

                if (checkFileExtension != ".txt" || checkFileExtension != ".zip")
                {
                    if (filetext != "EXIS" && StockStoreName != storeNumber && ActStockDate != exDate)
                    {
                        return new FileUplaodRespone
                        {
                            Success = false,
                            Error = "please choose correct file."
                        };
                    }

                    if (checkFileExtension == ".txt")
                    {
                        if (filetext == "EXIS" && StockStoreName == storeNumber && ActStockDate == exDate)
                        {
                            var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == StockStoreName && x.FileDate == ActStockDate && x.Category == tempfileName.Substring(0, 4).ToString());
                            if (filedata.Result != null)
                            {
                                return new FileUplaodRespone
                                {
                                    Success = false,
                                    Error = "file is already uploaded."
                                };
                            }
                            // condition 

                            var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                            fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.

                            var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");

                            if (!Directory.Exists(pathBuilt))
                            {
                                Directory.CreateDirectory(pathBuilt);
                            }

                            var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await model.File.CopyToAsync(stream);
                            }

                            var stockFile = File.ReadAllLines(path);
                            // for inner header
                            string stockInnerFileHeader = stockFile[0].Substring(0, 6).Trim();
                            string stockInnerFileName = stockFile[0].Substring(7, 11).Trim();
                            string stockInnerStoreNumber = stockFile[0].Substring(18, 4).Trim();
                            string stockInnerFileDate = stockFile[0].Substring(22, 7).Trim();
                            string stockInnerDate = string.Format("{0:ddMMyy}", stockInnerFileDate).Trim();

                            if (stockInnerFileName == "EXISTENCIA" && stockInnerStoreNumber == storeNumber && stockInnerDate == forInnerStockDate)
                            {
                                uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "Stock" } };
                                await _context.BulkInsertAsync(uploadFileNames);

                                //for file header
                                fileStores = stockFile.Take(1).SelectMany(y => new List<FileStore>
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

                                // file content
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
                                    FileStore fileStore = _context.FileStore.Where(u => u.Category == "Stock" && u.StoreNumber == model.StoreName.ToString() && u.FileDate == exDate).SingleOrDefault();
                                    fileStore.RecordCount = stockList.Count.ToString();
                                    await _context.SaveChangesAsync();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);

                                }
                                isSaveSuccess = true;
                            }
                        }
                    }
                }
                if (checkFileExtension == ".zip")
                {

                         //for zip file
                         string existingFile = model.File.FileName.ToString();
                
                        // var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                        // fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.
                        var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");
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

                        if (filetext == "EXIS" && StockStoreName == storeNumber && ActStockDate == exDate)
                            {
                            var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == StockStoreName && x.FileDate == ActStockDate && x.Category == "EXISTENCIA");
                            if (filedata.Result != null)
                            {
                                return new FileUplaodRespone
                                {
                                    Success = false,
                                    Error = "file is already uploaded."
                                };
                            }
                            var stockFile = File.ReadAllLines(destinationPath);
                            string regex = "^0+(?!$)";

                            string stockInnerFileHeader = stockFile[0].Substring(0, 6).Trim();
                            string stockInnerFileName = stockFile[0].Substring(7, 11).Trim();
                            string stockInnerStoreNumber = stockFile[0].Substring(18, 4).Trim();
                            string stockInnerFileDate = stockFile[0].Substring(22, 7).Trim();
                            string stockInnerDate = string.Format("{0:ddMMyy}", stockInnerFileDate).Trim();

                            if (stockInnerFileName == "EXISTENCIA" && stockInnerStoreNumber == storeNumber && stockInnerDate == forInnerStockDate)
                            {
                                uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "EXISTENCIA" } };
                                await _context.BulkInsertAsync(uploadFileNames);

                                //for file header
                                fileStores = stockFile.Take(1).SelectMany(y => new List<FileStore>
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
                            stockList = stockFile.Skip(1).SelectMany(x => new List<Stock>
                                    {
                                    new()
                                    {
                                    Store=Regex.Replace(x.Substring(0,4),regex,""),
                                    SKU=Regex.Replace(x.Substring(4,14),regex,""),
                                    Departament=Regex.Replace(x.Substring(18,4),regex,""),
                                    Description=Regex.Replace(x.Substring(22,30),regex,""),
                                    PrecVtaNorm=Regex.Replace(x.Substring(52,8),regex,""),
                                    PrecVtaNorm_SImpto=Regex.Replace(x.Substring(60,8),regex,""),
                                    SOH=Regex.Replace(x.Substring(68,12),regex,""),
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
                            // File.Delete(pathBuilt);
                            File.Delete(destinationPath);
                            FileStore fileStore = _context.FileStore.Where(u => u.Category == "Stock" && u.StoreNumber == model.StoreName.ToString() && u.FileDate == exDate).SingleOrDefault();
                            fileStore.RecordCount = stockList.Count.ToString();
                            await _context.SaveChangesAsync();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                        isSaveSuccess = true;    
                }
                }
            
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return new FileUplaodRespone
            {
                stockRecordCount = stockList.Count.ToString(),
                TimeElapsed = timeElapsed,
                Success = isSaveSuccess
            };
        }
        public async Task<FileUplaodRespone> ReservedData(FilterDataModel model)
        {
            bool isSaveSuccess = false;
            string fileName;



            List<Reserved> reservedList = new();



            //for file header
            List<FileStore> fileStores = new();
            //for file name
            List<UploadFileName> uploadFileNames = new();
            var stopwatch = new Stopwatch();
            stopwatch.Start();



            double timeElapsed = 0;



            try
            {
                string jobDate = model.StockDate.ToString();
                string jobOrderStore = model.StoreId.ToString();
                string jobOrderStoreName = model.StoreName.ToString();
                string jobOrderDate = String.Format("{0:MMyy}", model.StockDate);
                string forInnerStockDate = String.Format("{0:ddMMyy}", model.StockDate);



                var tempfileName = model.File.FileName;
                string filetext = tempfileName.Substring(0, 4);
                string fileDate = tempfileName.Substring(4, 4);
                string exDate = string.Format("{0:MMyy}", fileDate);
                string storeNumber = tempfileName.Substring(9, 4);
                var checkFileExtension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];



                if (checkFileExtension != ".txt" || checkFileExtension != ".zip")
                {
                    if (filetext != "APAR" && jobOrderStoreName != storeNumber && jobOrderDate != exDate)
                    {
                        return new FileUplaodRespone
                        {
                            Success = false,
                            Error = "please choose correct file."
                        };
                    }



                    if (checkFileExtension == ".txt")
                    {
                        if (filetext == "APAR" && jobOrderStoreName == storeNumber && jobOrderDate == exDate)
                        {
                            var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == jobOrderStoreName && x.FileDate == jobOrderDate);
                            if (filedata.Result != null)
                            {
                                return new FileUplaodRespone
                                {
                                    Success = false,
                                    Error = "file is already uploaded."
                                };
                            }
                            // condition



                            var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                            fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.



                            var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");



                            if (!Directory.Exists(pathBuilt))
                            {
                                Directory.CreateDirectory(pathBuilt);
                            }



                            var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await model.File.CopyToAsync(stream);
                            }



                            var reservedFile = File.ReadAllLines(path);
                            // for inner header
                            string stockInnerFileHeader = reservedFile[0].Substring(0, 6).Trim();
                            string stockInnerFileName = reservedFile[0].Substring(7, 11).Trim();
                            string stockInnerStoreNumber = reservedFile[0].Substring(18, 4).Trim();
                            string stockInnerFileDate = reservedFile[0].Substring(22, 7).Trim();
                            string stockInnerDate = string.Format("{0:ddMMyy}", stockInnerFileDate).Trim();



                            if (stockInnerFileName == "APARTADOS" && stockInnerStoreNumber == storeNumber && stockInnerDate == forInnerStockDate)
                            {
                                uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "Stock" } };
                                await _context.BulkInsertAsync(uploadFileNames);



                                //for file header
                                fileStores = reservedFile.Take(1).SelectMany(y => new List<FileStore>
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
                                string regex = "^0+(?!$)";
                                // file content
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
                        }
                    }
                }
                if (checkFileExtension == ".zip")
                {



                    //for zip file
                    string existingFile = model.File.FileName.ToString();



                    // var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                    // fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.
                    var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");
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



                    if (filetext == "APAR" && jobOrderStoreName == storeNumber && jobOrderDate == exDate)
                    {
                        var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == jobOrderStoreName && x.FileDate == jobOrderDate);
                        if (filedata.Result != null)
                        {
                            return new FileUplaodRespone
                            {
                                Success = false,
                                Error = "file is already uploaded."
                            };
                        }
                        var reservedFile = File.ReadAllLines(destinationPath);
                        string regex = "^0+(?!$)";



                        string stockInnerFileHeader = reservedFile[0].Substring(0, 6).Trim();
                        string stockInnerFileName = reservedFile[0].Substring(7, 11).Trim();
                        string stockInnerStoreNumber = reservedFile[0].Substring(18, 4).Trim();
                        string stockInnerFileDate = reservedFile[0].Substring(22, 7).Trim();
                        string stockInnerDate = string.Format("{0:ddMMyy}", stockInnerFileDate).Trim();



                        if (stockInnerFileName == "APARTADOS" && stockInnerStoreNumber == storeNumber && stockInnerDate == forInnerStockDate)
                        {
                            uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "Stock" } };
                            await _context.BulkInsertAsync(uploadFileNames);



                            //for file header
                            fileStores = reservedFile.Take(1).SelectMany(y => new List<FileStore>
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
                            // File.Delete(pathBuilt);
                            File.Delete(destinationPath);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    isSaveSuccess = true;
                }
            }



            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return new FileUplaodRespone
            {
                stockRecordCount = reservedList.Count.ToString(),
                TimeElapsed = timeElapsed,
                Success = isSaveSuccess
            };
        }
        public async Task<FileUplaodRespone> CatergoriesData(FilterDataModel model)
        {
            bool isSaveSuccess = false;
            string fileName;



            List<Categories> catergoriesList = new();
            //for file header
            List<FileStore> fileStores = new();
            //for file name
            List<UploadFileName> uploadFileNames = new();
            var stopwatch = new Stopwatch();
            stopwatch.Start();



            double timeElapsed = 0;



            try
            {
                string jobDate = model.StockDate.ToString();
                string jobOrderStore = model.StoreId.ToString();
                string jobOrderStoreName = model.StoreName.ToString();
                string jobOrderDate = String.Format("{0:MMyy}", model.StockDate);
                string forInnerStockDate = String.Format("{0:ddMMyy}", model.StockDate);



                var tempfileName = model.File.FileName;
                string filetext = tempfileName.Substring(0, 4);
                string fileDate = tempfileName.Substring(4, 4);
                string exDate = string.Format("{0:MMyy}", fileDate);
                string storeNumber = tempfileName.Substring(9, 4);
                var checkFileExtension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];



                if (checkFileExtension != ".txt" || checkFileExtension != ".zip")
                {
                    if (filetext != "CATE" && jobOrderStoreName != storeNumber && jobOrderDate != exDate)
                    {
                        return new FileUplaodRespone
                        {
                            Success = false,
                            Error = "please choose correct file."
                        };
                    }



                    if (checkFileExtension == ".txt")
                    {
                        if (filetext == "CATE" && jobOrderStoreName == storeNumber && jobOrderDate == exDate)
                        {
                            var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == jobOrderStoreName && x.FileDate == jobOrderDate);
                            if (filedata.Result != null)
                            {
                                return new FileUplaodRespone
                                {
                                    Success = false,
                                    Error = "file is already uploaded."
                                };
                            }
                            // condition



                            var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                            fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.



                            var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");



                            if (!Directory.Exists(pathBuilt))
                            {
                                Directory.CreateDirectory(pathBuilt);
                            }



                            var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await model.File.CopyToAsync(stream);
                            }



                            var catergoriesFile = File.ReadAllLines(path);
                            // for inner header
                            string stockInnerFileHeader = catergoriesFile[0].Substring(0, 6).Trim();
                            string stockInnerFileName = catergoriesFile[0].Substring(7, 11).Trim();
                            string stockInnerStoreNumber = catergoriesFile[0].Substring(18, 4).Trim();
                            string stockInnerFileDate = catergoriesFile[0].Substring(22, 7).Trim();
                            string stockInnerDate = string.Format("{0:ddMMyy}", stockInnerFileDate).Trim();



                            if (stockInnerFileName == "CATE DIV" && stockInnerStoreNumber == storeNumber && stockInnerDate == forInnerStockDate)
                            {
                                uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "Stock" } };
                                await _context.BulkInsertAsync(uploadFileNames);



                                //for file header
                                fileStores = catergoriesFile.Take(1).SelectMany(y => new List<FileStore>
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
                                string regex = "^0+(?!$)";
                                // file content
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
                        }
                    }
                }
                if (checkFileExtension == ".zip")
                {



                    //for zip file
                    string existingFile = model.File.FileName.ToString();



                    // var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                    // fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.
                    var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");
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



                    if (filetext == "CATE" && jobOrderStoreName == storeNumber && jobOrderDate == exDate)
                    {
                        var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == jobOrderStoreName && x.FileDate == jobOrderDate);
                        if (filedata.Result != null)
                        {
                            return new FileUplaodRespone
                            {
                                Success = false,
                                Error = "file is already uploaded."
                            };
                        }
                        var categoriesFile = File.ReadAllLines(destinationPath);
                        string regex = "^0+(?!$)";



                        string stockInnerFileHeader = categoriesFile[0].Substring(0, 6).Trim();
                        string stockInnerFileName = categoriesFile[0].Substring(7, 11).Trim();
                        string stockInnerStoreNumber = categoriesFile[0].Substring(18, 4).Trim();
                        string stockInnerFileDate = categoriesFile[0].Substring(22, 7).Trim();
                        string stockInnerDate = string.Format("{0:ddMMyy}", stockInnerFileDate).Trim();



                        if (stockInnerFileName == "CATE DIV" && stockInnerStoreNumber == storeNumber && stockInnerDate == forInnerStockDate)
                        {
                            uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "Stock" } };
                            await _context.BulkInsertAsync(uploadFileNames);



                            //for file header
                            fileStores = categoriesFile.Take(1).SelectMany(y => new List<FileStore>
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
                        catergoriesList = categoriesFile.Skip(1).SelectMany(x => new List<Categories>
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
                            // File.Delete(pathBuilt);
                            File.Delete(destinationPath);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    isSaveSuccess = true;
                }
            }



            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return new FileUplaodRespone
            {
                stockRecordCount = catergoriesList.Count.ToString(),
                TimeElapsed = timeElapsed,
                Success = isSaveSuccess
            };



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
            //for file header
            List<FileStore> fileStores = new();
            //for file name
            List<UploadFileName> uploadFileNames = new();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            double timeElapsed = 0;
            try
            {
                string jobDate = model.StockDate.ToString();
                string jobOrderStore = model.StoreId.ToString();
                string jobOrderStoreName = model.StoreName.ToString();
                string jobOrderDate = String.Format("{0:MMyy}", model.StockDate);
                string forInnerStockDate = String.Format("{0:ddMMyy}", model.StockDate);

                var tempfileName = model.File.FileName;
                string filetext = tempfileName.Substring(0, 4);
                string fileDate = tempfileName.Substring(4, 4);
                string exDate = string.Format("{0:MMyy}", fileDate);
                string storeNumber = tempfileName.Substring(9, 4);
                var checkFileExtension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];

                if (checkFileExtension != ".txt" || checkFileExtension != ".zip")
                {
                    if (filetext != "MAST" && jobOrderStoreName != storeNumber && jobOrderDate != exDate)
                    {
                        return new FileUplaodRespone
                        {
                            Success = false,
                            Error = "please choose correct file."
                        };
                    }

                    if (checkFileExtension == ".txt")
                    {
                        if (filetext == "MAST" && jobOrderStoreName == storeNumber && jobOrderDate == exDate)
                        {
                            var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == jobOrderStoreName && x.FileDate == jobOrderDate && x.Category== "OrderJob");
                            if (filedata.Result != null)
                            {
                                return new FileUplaodRespone
                                {
                                    Success = false,
                                    Error = "file is already uploaded."
                                };
                            }
                            // condition 

                            var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                            fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.

                            var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");

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
                            string jobOrderInnerDate = string.Format("{0:ddMMyy}", jobOrderInnerFileDate).Trim();

                            if (jobOrderInnerFileName == "MASTER" && jobOrderInnerStoreNumber == storeNumber && jobOrderInnerDate == forInnerStockDate)
                            {
                                uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "OrderJob" } };
                                await _context.BulkInsertAsync(uploadFileNames);

                                //for file header
                                fileStores = orderJobFile.Take(1).SelectMany(y => new List<FileStore>
                                    {
                                    new()
                                    {
                                        Header=(y.Substring(0,6)),
                                        FileName=(y.Substring(7,7)),
                                        StoreNumber=(y.Substring(18,4)),
                                        FileDate=string.Format("{0:MMyy}",y.Substring(24,4)),
                                        Category="OrderJob",
                                    }}).ToList();
                                await _context.BulkInsertAsync(fileStores);

                                // file content
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
                                    FileStore fileStore = _context.FileStore.Where(u => u.Category == "OrderJob" && u.StoreNumber == model.StoreName.ToString() && u.FileDate == jobOrderDate).SingleOrDefault();
                                    fileStore.RecordCount = orderJobList.Count.ToString();
                                    await _context.SaveChangesAsync();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);

                                }
                                isSaveSuccess = true;
                            }
                        }
                    }
                }
                if (checkFileExtension == ".zip")
                {

                    //for zip file
                    string existingFile = model.File.FileName.ToString();

                    // var extension = "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                    // fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.
                    var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");
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

                    if (filetext == "MAST" && jobOrderStoreName == storeNumber && jobOrderDate == exDate)
                    {
                        var filedata = _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber == jobOrderStoreName && x.FileDate == jobOrderDate && x.Category == filetext);
                        if (filedata.Result != null)
                        {
                            return new FileUplaodRespone
                            {
                                Success = false,
                                Error = "file is already uploaded."
                            };
                        }
                        var zipOrderJobFile = File.ReadAllLines(destinationPath);
                        string regex = "^0+(?!$)";

                        string jobOrderInnerFileHeader = zipOrderJobFile[0].Substring(0, 6).Trim();
                        string jobOrderInnerFileName = zipOrderJobFile[0].Substring(7, 11).Trim();
                        string jobOrderInnerStoreNumber = zipOrderJobFile[0].Substring(18, 4).Trim();
                        string jobOrderInnerFileDate = zipOrderJobFile[0].Substring(22, 7).Trim();
                        string jobOrderInnerDate = string.Format("{0:ddMMyy}", jobOrderInnerFileDate).Trim();

                        if (jobOrderInnerFileName == "MASTER" && jobOrderInnerStoreNumber == storeNumber && jobOrderInnerDate == forInnerStockDate)
                        {
                            uploadFileNames = new List<UploadFileName> { new() { FileName = tempfileName.Substring(0, 4), StoreNumber = tempfileName.Substring(4, 4), FileDate = tempfileName.Substring(9, 4), Category = "OrderJob" } };
                            await _context.BulkInsertAsync(uploadFileNames);

                            //for file header
                            fileStores = zipOrderJobFile.Take(1).SelectMany(y => new List<FileStore>
                                    {
                                    new()
                                    {
                                    Header=(y.Substring(0,6)),
                                    FileName=(y.Substring(7,11)),
                                    StoreNumber=(y.Substring(18,4)),
                                    FileDate=string.Format("{0:MMyy}",y.Substring(24,4)),
                                    Category="OrderJob",
                                    }}).ToList();
                            await _context.BulkInsertAsync(fileStores);
                        }
                        orderJobList = zipOrderJobFile.Skip(1).SelectMany(x => new List<OrderJob>
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
                            // File.Delete(pathBuilt);
                            File.Delete(destinationPath);
                            FileStore fileStore = _context.FileStore.Where(u => u.Category==filetext  && u.StoreNumber== model.StoreName.ToString() && u.FileDate== jobOrderDate).SingleOrDefault();
                            fileStore.RecordCount = orderJobList.Count.ToString();
                            await _context.SaveChangesAsync();

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    isSaveSuccess = true;
                }
            }

            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return new FileUplaodRespone
            {
                stockRecordCount = orderJobList.Count.ToString(),
                TimeElapsed = timeElapsed,
                Success = isSaveSuccess
            };
        }

    }
}
