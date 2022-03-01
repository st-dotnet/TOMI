using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly Logger logger;
        private readonly TOMIDataContext _context;
   
        public ReportOptionRepository(TOMIDataContext context, IMapper mapper)
        {
            logger = LogManager.GetCurrentClassLogger();
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<StockAdjustment>> GetCodeNotFoundAsync()
        {
            return await _context.StockAdjustment.Include(x => x.OrderJob).ToListAsync();
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
                             join a in _context.Stock on b.Department equals a.Departament
                             select new stockandorder
                             {
                                 SKU = a.SKU,
                                 Description = a.Description,
                                 SOH = a.SOH,
                                 PrecVtaNorm = a.PrecVtaNorm,
                                 Code = b.Code
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
                             join c in _context.OrderJob on a.Departament equals c.Department
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
                                 Id = b.Id,
                                 Department = a.Departament
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
        public async Task<FileUplaodRespone> InventoryFigure(FilterCustomerReportDataModel model)
        {
            string fileName=string.Empty;
            string InnerHeaderName=string.Empty;
            string innerFooter=string.Empty;
            var innerDataError =  model.StockDate.Value.Date.Month.ToString("#00")+ model.StockDate.Value.Date.Day.ToString("#00");
            var figureStoreDate = model.StoreName.ToString().Substring(0, 4);
            var InnerContentDate= model.StockDate.Value.Date.Day.ToString("#00")+ model.StockDate.Value.Date.Month.ToString("#00")+model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
            string numberOfRecords = "000000.";
            fileName = "CTLR"+innerDataError+"_"+ figureStoreDate + ".txt";
            InnerHeaderName = "HEADER" + " " + "CIFRAS INV" +figureStoreDate+InnerContentDate + " "+ numberOfRecords;

            innerFooter = "TRAILER" + " " + "CIFRAS INV" + figureStoreDate + InnerContentDate;

            var query = (from b in _context.OrderJob
                         join a in _context.StockAdjustment on b.Code equals a.Barcode
                         select new InventoryFigure
                         {
                             StoreNumber = b.Store,
                             FigureDate = b.StockDate,
                             Unit = (int)a.Quantity,
                             Amount = b.SalePrice,
                         }).ToList();
            
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            File.Create(fileName).Close();
            string line = string.Join(",", InnerHeaderName) + System.Environment.NewLine;
            foreach (var item in query)
            {
                var currentUnits = item.Unit.ToString();
                var currentAmount = item.Amount.ToString();
                for (int i = 0; i < 12; i++)
                {
                    if (currentUnits.Length < 12)
                        currentUnits = "0" + currentUnits;
                }
                for (int i = 0; i < 12; i++)
                {
                    if (currentAmount.Length < 12)
                        currentAmount = "0" + currentAmount;
                }
                var date= item.FigureDate.Value.Date.Year.ToString().Substring(0, 4).ToString()
                         + item.FigureDate.Value.Date.Month.ToString("#00")
                         + item.FigureDate.Value.Date.Day.ToString("#00");
                line = line + string.Join(",", item.StoreNumber+ date + currentUnits + currentAmount) + System.Environment.NewLine;
            }

            var currentCountRec = query.Count.ToString();
            
            for (int i = 0; i < 6; i++)
            {
                if (currentCountRec.Length < 6)
                    currentCountRec = "0" + currentCountRec;
            }

            line = line + string.Join(",", innerFooter +" "+ currentCountRec) + System.Environment.NewLine;

            File.AppendAllText(fileName, line);

            return null;
        }

        public Task<FileUplaodRespone> InventoryDetail(FilterCustomerReportDataModel model)
        {
            string fileName = string.Empty;
            string InnerHeaderName = string.Empty;
            string innerFooter = string.Empty;
            var innerDataError = model.StockDate.Value.Date.Month.ToString("#00") + model.StockDate.Value.Date.Day.ToString("#00");
            var figureStoreDate = model.StoreName.ToString().Substring(0, 4);
            var InnerContentDate = model.StockDate.Value.Date.Day.ToString("#00") + model.StockDate.Value.Date.Month.ToString("#00") + model.StockDate.Value.Date.Year.ToString().Substring(2, 2).ToString();
            string numberOfRecords = "000000.";
            fileName = "INVR" + innerDataError + "_" + figureStoreDate + ".txt";
            InnerHeaderName = "HEADER" + " " + "INVENTARIO" + figureStoreDate + InnerContentDate + " " + numberOfRecords;

            innerFooter = "TRAILER" + " " + "INVENTARIO" + figureStoreDate + InnerContentDate;

            var query = (from b in _context.OrderJob
                         join a in _context.StockAdjustment on b.Code equals a.Barcode
                         select new InventoryFigure
                         {
                             StoreNumber = b.Store,
                             FigureDate = b.StockDate,
                             Unit = (int)a.Quantity,
                             Amount = b.SalePrice,
                         }).ToList();

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            File.Create(fileName).Close();
            string line = string.Join(",", InnerHeaderName) + System.Environment.NewLine;
            foreach (var item in query)
            {
                var currentUnits = item.Unit.ToString();
                var currentAmount = item.Amount.ToString();
                for (int i = 0; i < 12; i++)
                {
                    if (currentUnits.Length < 12)
                        currentUnits = "0" + currentUnits;
                }
                for (int i = 0; i < 12; i++)
                {
                    if (currentAmount.Length < 12)
                        currentAmount = "0" + currentAmount;
                }
                var date = item.FigureDate.Value.Date.Year.ToString().Substring(0, 4).ToString()
                         + item.FigureDate.Value.Date.Month.ToString("#00")
                         + item.FigureDate.Value.Date.Day.ToString("#00");
                line = line + string.Join(",", item.StoreNumber + date + currentUnits + currentAmount) + System.Environment.NewLine;
            }

            var currentCountRec = query.Count.ToString();

            for (int i = 0; i < 6; i++)
            {
                if (currentCountRec.Length < 6)
                    currentCountRec = "0" + currentCountRec;
            }

            line = line + string.Join(",", innerFooter + " " + currentCountRec) + System.Environment.NewLine;

            File.AppendAllText(fileName, line);

            return null;
        }
    }
}

