using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog;
using TOMI.Data.Database;
using TOMI.Data.Database.Entities;
using TOMI.Services.Interfaces;
using TOMI.Services.Models;

namespace TOMI.Services.Repository
{
    public class ReportOptionRepository : IReportOptionService
    {
        private readonly IMapper _mapper;
      //  private readonly Logger logger;
        private readonly TOMIDataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReportOptionRepository(
            TOMIDataContext context,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment)
        {
           // logger = LogManager.GetCurrentClassLogger();
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<List<StockAdjustment>> GetCodeNotFoundAsync()
        {
              return await _context.StockAdjustment.Include(x => x.OrderJob).Where(c => c.NOF == 1).ToListAsync();
           // return  await _context.spCodeNotfoundReport.FromSqlRaw("EXECUTE dbo.spgetCodeNotFoundInFile").ToListAsync();
        }
        public async Task<List<StockAdjustment>> GetLabelDetailsAsync(int? tagFrom, int? tagTo)
        {
            if (tagFrom != 0 && tagTo != 0)
            {
                return await _context.StockAdjustment.Include(x => x.OrderJob).Where(x => x.Tag >= tagFrom && x.Tag <= tagTo).OrderBy(x => x.Rec).ToListAsync();
            }
            else
            {
                return await _context.StockAdjustment.Include(x => x.OrderJob).OrderBy(x => x.Rec).ToListAsync();
            }
        }
        //  public async Task<List<StockAdjustment>> GetLabelDetailsAsync()
        //   {
        //     return await _context.StockAdjustment.Include(x => x.OrderJob).OrderBy(x => x.Rec).ToListAsync();
        // var customerId = new SqlParameter("@customerId", model.CustomerId);
        //  return  await _context.StockAdjustment.FromSqlRaw("EXECUTE dbo.DetailLabelRecords").ToListAsync();
        //}
        public async Task<List<StockAdjustment>> GetExtendedPricesAsync()
        {
            return await _context.StockAdjustment.Include(x => x.OrderJob).OrderBy(x => x.Tag).ToListAsync();
        }
        public List<stockandorder> GetUncountedItemsAsync()
        {
            try
            {
                var query = (from b in _context.OrderJob
                             join a in _context.Stock on b.Department equals a.Department
                             select new stockandorder
                             {
                                 SKU = a.SKU,
                                 Description = a.Description,
                                 SOH = a.SOH,
                                 PrecVtaNorm = a.PrecVtaNorm,
                                 Code = b.Code,
                                 Department=a.Department
                                 
                             }).Take(500).ToList();
                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public List<StockAndStockAdjust> GetVariationBySKUAsync()
        {
            try
            {
                var query = (from a in _context.Stock
                             join c in _context.OrderJob on a.Department equals c.Department
                             join b in _context.StockAdjustment on c.Id equals b.SKU

                             select new StockAndStockAdjust
                             {
                                 SKU = a.SKU,
                                 Description = a.Description,
                                 SOH = a.SOH,
                                 PrecVtaNorm = a.PrecVtaNorm,
                                 Quantity = (int)b.Quantity,
                                 Barcode = b.Barcode,
                                 Tag = (int)b.Tag,
                                 Rec = b.Rec,
                                 Department = a.Department
                             }).Take(20).ToList();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<List<StockAdjustment>> GetCorrectionsReportAsync()
        {
            return await _context.StockAdjustment.Include(x => x.OrderJob).OrderBy(x => x.Tag).Take(500).ToListAsync();
        }
        public List<DeptAndStockAdjust> GetBreakDownReportAsync()
        {
            try
            {
                var query = (from a in _context.Departments
                             join c in _context.OrderJob on a.Department equals c.Department
                             join b in _context.StockAdjustment on c.Id equals b.SKU
                             orderby a.Division
                             select new DeptAndStockAdjust
                             {
                                 Department = b.Department,
                                 DepartmentName = a.DepartmentName,
                                 Quantity = b.Quantity,
                                 SalePrice = c.SalePrice,
                                 PriceWithoutTaxes = c.PriceWithoutTaxes
                             }).Take(20).ToList();
                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<List<StockAdjustment>> GetDateTimeCheckReport()
        {
            return await _context.StockAdjustment.Include(x => x.OrderJob).OrderBy(x => x.Tag).Take(500).ToListAsync();
            // return await _context.StockAdjustment.Include(x => x.OrderJob).Where(x => x.CreatedAt >= fromDate && x.CreatedAt <= toDate).OrderBy(x => x.Rec).ToListAsync();
        }
        public async Task<List<Departments>> GetDepartments()
        {
            return await _context.Departments.OrderBy(x => x.DepartmentName).ToListAsync();
        }

        public async Task<List<Ranges>> GetRangesforReport()
        {
            return await _context.Ranges.OrderBy(x => x.Id).ToListAsync();
        }

        public Task<List<StockAdjustment>> GetLabelDetailsAsyncTest()
        {
            throw new NotImplementedException();
        }
        public async Task<string> InventoryFigure(FilterCustomerReportDataModel model)
        {
            string fileName = string.Empty;
            string InnerHeaderName = string.Empty;
            string innerFooter = string.Empty;
            var innerDataError = model.StockDate.Value.Date.Month.ToString("#00") + model.StockDate.Value.Date.Day.ToString("#00");
            var figureStoreDate = model.StoreName.ToString().Substring(0, 4);
            var InnerContentDate = model.StockDate.Value.Date.Day.ToString("#00") + model.StockDate.Value.Date.Month.ToString("#00") + model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
            string numberOfRecords = "000000.";
            fileName = "CTLR" + innerDataError + "_" + figureStoreDate + ".txt";
            InnerHeaderName = "HEADER" + "  " + "CIFRAS INV" + figureStoreDate + InnerContentDate + " " + numberOfRecords;

            innerFooter = "TRAILER" + " " + "CIFRAS INV" + figureStoreDate + InnerContentDate;

             var query = await _context.getInventoryFigureData.FromSqlRaw("EXECUTE dbo.spgetInventoryFigureData").ToListAsync();

            string webRootPath = _webHostEnvironment.WebRootPath;
            var path = Path.Combine($"{webRootPath}\\Upload", fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.Create(path).Close();

            string line = string.Join(",", InnerHeaderName) + System.Environment.NewLine;
            foreach (var item in query)
            {
                line = line + string.Join(",", item.StoreName + InnerContentDate + item.Qty + item.Amount) + System.Environment.NewLine;
            }

            var currentCountRec = query.Count.ToString();

            for (int i = 0; i < 6; i++)
            {
                if (currentCountRec.Length < 6)
                    currentCountRec = "0" + currentCountRec;
            }

            line = line + string.Join(",", innerFooter + " " + currentCountRec) + System.Environment.NewLine;

            File.AppendAllText(path, line);

            return path;
        }

        public async Task<string> InventoryDetail(FilterCustomerReportDataModel model)
        {
            string fileName = string.Empty;
            string InnerHeaderName = string.Empty;
            string innerFooter = string.Empty;
            string InnerTime = string.Empty;


            InnerTime = model.StockDate.Value.TimeOfDay.Hours.ToString("#00") + model.StockDate.Value.TimeOfDay.Minutes.ToString("#00") + model.StockDate.Value.TimeOfDay.Seconds.ToString("#00");

            //       + model.StockDate.Value.TimeOfDay.Minutes.ToString("#00") + model.StockDate.Value.Date.TimeOfDay.Seconds.ToString("#00");
            var innerDataError = model.StockDate.Value.Date.Month.ToString("#00") + model.StockDate.Value.Date.Day.ToString("#00");
            var InventoryStoreDate = model.StoreName.ToString().Substring(0, 4);
            var InnerContentDate = model.StockDate.Value.Date.Day.ToString("#00") + model.StockDate.Value.Date.Month.ToString("#00") + model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
            string numberOfRecords = "000000.";

            fileName = "INVR" + innerDataError + "_" + InventoryStoreDate + ".txt";
            InnerHeaderName = "HEADER" + " " + "INVENTARIO" + InventoryStoreDate + InnerContentDate + " " + numberOfRecords;
            innerFooter = "TRAILER" + " " + "INVENTARIO" + InventoryStoreDate + InnerContentDate;

            var query = await _context.getInventarios.FromSqlRaw("EXECUTE dbo.spgetInventarios").ToListAsync();

            string webRootPath = _webHostEnvironment.WebRootPath;
            var path = Path.Combine($"{webRootPath}\\Upload", fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.Create(path).Close();

            string line = string.Join(",", InnerHeaderName) + System.Environment.NewLine;
            foreach (var item in query)
            {
               
                line = line + string.Join(",", item.Store + item.Zona + item.Tag + item.Consecutive + item.Code + item.Department + item.Qty + item.Price + item.Price_Indicator+ InnerContentDate + InnerTime + ".") + System.Environment.NewLine;
            }
            var currentCountRec = query.Count.ToString();

            for (int i = 0; i < 6; i++)
            {
                if (currentCountRec.Length < 6)
                    currentCountRec = "0" + currentCountRec;
            }
            line = line + string.Join(",", innerFooter + " " + currentCountRec) + System.Environment.NewLine;

            File.AppendAllText(path, line);
            
            return path;
        }
        public async Task<string> MarbeteDetail(FilterCustomerReportDataModel model)
        {
            string fileName = string.Empty;
            string InnerHeaderName = string.Empty;
            string innerFooter = string.Empty;
            string InnerTime = string.Empty;


            InnerTime = model.StockDate.Value.TimeOfDay.Hours.ToString("#00") + model.StockDate.Value.TimeOfDay.Minutes.ToString("#00") + model.StockDate.Value.TimeOfDay.Seconds.ToString("#00");

            //       + model.StockDate.Value.TimeOfDay.Minutes.ToString("#00") + model.StockDate.Value.Date.TimeOfDay.Seconds.ToString("#00");
            var innerDataError = model.StockDate.Value.Date.Month.ToString("#00") + model.StockDate.Value.Date.Day.ToString("#00");
            var InventoryStoreDate = model.StoreName.ToString().Substring(0, 4);
            var InnerContentDate = model.StockDate.Value.Date.Day.ToString("#00") + model.StockDate.Value.Date.Month.ToString("#00") + model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
            string numberOfRecords = "000000.";
            fileName = "MARB" + innerDataError + "_" + InventoryStoreDate + ".txt";
            InnerHeaderName = "HEADER" + "  " + "MARBETE" + InventoryStoreDate + InnerContentDate + " " + numberOfRecords;
            innerFooter = "TRAILER" + " " + "MARBETE" + InventoryStoreDate + InnerContentDate;

            var query = await _context.getMarbetes.FromSqlRaw("EXECUTE dbo.spgetMarbeteDetail").ToListAsync();
            string webRootPath = _webHostEnvironment.WebRootPath;
            var path = Path.Combine($"{webRootPath}\\Upload", fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.Create(path).Close();            

            string line = string.Join(",", InnerHeaderName) + System.Environment.NewLine;
            foreach (var item in query)
            {
                line = line + string.Join(",", item.Zona + item.Tag + item.Department+"......................") + System.Environment.NewLine;
            }
            

            var currentCountRec = query.Count.ToString();

            for (int i = 0; i < 6; i++)
            {
                if (currentCountRec.Length < 6)
                    currentCountRec = "0" + currentCountRec;
            }
            line = line + string.Join(",", innerFooter + " " + currentCountRec) + System.Environment.NewLine;

            File.AppendAllText(path, line);

            return path;


        }

        public async Task<List<BillingReport>> GetBillingReport()
        {
             return  await _context.billingReports.FromSqlRaw("EXECUTE dbo.spGenerateBillingReport").ToListAsync();
            
        }
    }
}

