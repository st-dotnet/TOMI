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

        public async Task<List<StockAdjustment>> GetLabelDetailsAsync()
        {
            return await _context.StockAdjustment.Include(x => x.OrderJob).OrderBy(x => x.Rec).ToListAsync();

        }

        public async Task<List<StockAdjustment>> GetExtendedPricesAsync()
        {
            return await _context.StockAdjustment.Include(x => x.OrderJob).OrderBy(x => x.Tag).ToListAsync();
        }

        //public List<Stock> GetUncountedItemsAsync()
        //{
        //    var query = (from a in _context.Stock
        //                 group a by a.SKU into pg
        //                 join b in _context.OrderJob on pg.FirstOrDefault().SKU equals b.SKU
        //                 select new
        //                 {
        //                     pg.FirstOrDefault().SKU,
        //                     pg.FirstOrDefault().Description,
        //                     pg.FirstOrDefault().SOH,
        //                     pg.FirstOrDefault().PrecVtaNorm,
        //                     b.Code
        //                 }).ToList();
           
        //    return query.ToList();
        //}

        public List<stockandorder> GetUncountedItemsAsync()
        {
            try
            {
                var query = (from b in _context.OrderJob
                                // group a by a.SKU into pg
                            join a in _context.Stock on b.Department equals a.Departament
                            // where b.Department=="128"
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
                throw ex;
            }
        }

        //public async Task<List<Stock>> GetVariationBySKUAsync()
        //{
        //    var query = (from a in _context.Stock
        //                 join b in _context.StockAdjustment on a.Id equals b.SKU

        //                 select new
        //                 {
        //                     a.SKU,
        //                     a.Description,
        //                     a.SOH,
        //                     a.PrecVtaNorm,
        //                     b.Quantity,
        //                     b.Barcode,
        //                     b.Tag,
        //                     b.Id
        //                 }).ToList();
        //    return (List<Stock>)query;

        //}



        public async task<list<stockadjustment>> getcorrectionsreportasync()
        {
            return await _context.stockadjustment.include(x => x.orderjob).orderby(x => x.tag).tolistasync();
        }
        //public async Task<List<StockAdjustment>> GetLabelDetailsAsync(FilterDataModel filterDataModel)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
