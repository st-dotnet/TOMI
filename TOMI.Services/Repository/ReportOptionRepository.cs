using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TOMI.Data.Database;
using TOMI.Data.Database.Entities;
using TOMI.Services.Interfaces;
using TOMI.Services.Models;

namespace TOMI.Services.Repository
{
    public class ReportOptionRepository : IReportOptionService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ReportOptionRepository> _logger;
        private readonly TOMIDataContext _context;
        public ReportOptionRepository(ILogger<ReportOptionRepository> logger, TOMIDataContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<StockAdjustment>> GetCodeNotFoundAsync()
        {
            return await _context.StockAdjustment.Include(x => x.OrderJob).ToListAsync();
        }
        public async Task<List<StockAdjustment>> GetLabelDetailsAsync(int? tagFrom,int? tagTo )
        {
            if (tagFrom != 0 && tagTo!=0)
            {
                return await _context.StockAdjustment.Include(x => x.OrderJob).Where(x=>x.Tag >= tagFrom && x.Tag <= tagTo).OrderBy(x => x.Rec).ToListAsync();
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
                                     Department=a.Departament
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

        public async Task<List<StockAdjustment>> GetAdjustmentReport()
        {
            return await _context.StockAdjustment.Where(x => x.Isdeleted == true).ToListAsync();
        }

        public List<OrderAndStockAdjust> InventoryNumberFile()
        {
            try
            {
                var query = (from a in _context.OrderJob
                             join b in _context.StockAdjustment on a.Code equals b.Barcode
                             select new OrderAndStockAdjust
                             {
                                 Store = a.Store,
                                 InventoryDate = a.StockDate,
                                 SalePrice = a.SalePrice,
                                 Quantity = b.Quantity
                             }).Take(20).ToList();
                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public List<StockAdjustAndOrder> DetailOfInventoriesFile()
        {
            try
            {
                var query = (from a in _context.OrderJob
                             join b in _context.StockAdjustment on a.Code equals b.Barcode
                             orderby b.Tag
                             select new StockAdjustAndOrder
                             {
                                 Store = a.Store,
                                 Tag=b.Tag,
                                 Barcode=b.Barcode,
                                 Department=b.Department,
                                 Quantity = b.Quantity,
                                 SalePrice=a.SalePrice,
                                 CreatedAt=b.CreatedAt
                             }).Take(20).ToList();
                return query;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

    }
}

