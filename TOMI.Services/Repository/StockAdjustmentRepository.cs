using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class StockAdjustmentRepository : IStockAdjustmentService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<StockAdjustmentRepository> _logger;
        private readonly TOMIDataContext _context;
        public StockAdjustmentRepository(ILogger<StockAdjustmentRepository> logger, TOMIDataContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }
        public async Task<StockAdjustmentResponse> SaveStockAdjustment(StockAdjustmentModel model)
        {
            StockAdjustment stockAdjustment = await _context.StockAdjustment.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (stockAdjustment == null)
            {
                var stockadjustment = _mapper.Map<StockAdjustment>(model);
                StockAdjustment result = _context.StockAdjustment.Add(stockadjustment).Entity;
                await _context.SaveChangesAsync();
                return new StockAdjustmentResponse { Adjustment = result, Success = true };
            }
            else
            {
                var res = _mapper.Map<StockAdjustment>(model);
                stockAdjustment.Rec = model.Rec;
                stockAdjustment.Term = model.Term;
                stockAdjustment.Dload = model.Dload;
                stockAdjustment.Tag = model.Tag;
                stockAdjustment.Shelf = model.Shelf;
                stockAdjustment.Barcode = model.Barcode;
                stockAdjustment.SKU = model.SKU;
                _context.StockAdjustment.Update(stockAdjustment);
                await _context.SaveChangesAsync();
                return new StockAdjustmentResponse { Adjustment = res, Success = true };
            }
            throw new ValidationException("Data not found!");
        }

        public async Task<StockAdjustment> DeleteStockAdjustment(Guid id)
        {
            var existingRanges = await _context.StockAdjustment.FirstOrDefaultAsync(x => x.Id == id);
            if (existingRanges.Isdeleted == false)
            {
                existingRanges.Isdeleted = true;

                _context.SaveChanges();
            }
            return existingRanges;
        }

        public async Task<StockAdjustment> GetStockAdjustmentAsync(Guid id)
        {
            return await _context.StockAdjustment.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<StockAdjustment>> GetStockAdjustmentListAsync()
        {
            return await _context.StockAdjustment.Include(x => x.Master).Where(x => !x.Isdeleted).ToListAsync();
        }

        public async Task<List<StockAdjustment>> GoToRecord(int recId)
        {
            List<StockAdjustment> records = new();
            StockAdjustment recid = await _context.StockAdjustment.Include(x => x.Master).FirstOrDefaultAsync(x => x.Rec == recId);
            if (recid != null)
            {
                records.Add(recid);
            }
            return records;
        }

        public async Task<List<StockAdjustment>> GetDeletedRecord()
        {
            return await _context.StockAdjustment.Include(x => x.Master).Where(x => x.Isdeleted).ToListAsync();
        }

        public async Task<List<StockAdjustment>> ChangeDeletedRecStatus(int recid)
        {

            var toBeDeleted = await _context.StockAdjustment.Where(x => x.Rec == recid && x.Isdeleted == true).ToListAsync();

            toBeDeleted.ForEach(a => { a.Isdeleted = false; });

            await _context.SaveChangesAsync();

            return toBeDeleted;


        }

        public async Task<Master> MasterDataBySku(string sku)
        {
            return await _context.Master.FirstOrDefaultAsync(x => x.SKU == sku);
        }

        public async Task<List<StockAdjustment>> FilterStock(StockAdjustmentFilterModel model)
        {
            var stockAdjustmentData = await _context.StockAdjustment.Include(x => x.Master).ToListAsync();

            if (!string.IsNullOrEmpty(model.Department.ToString()))
                stockAdjustmentData = stockAdjustmentData.Where(s => s.Department == model.Department).ToList();

            if (model.Dload != null)
                stockAdjustmentData = stockAdjustmentData.Where(s => s.Dload == model.Dload).ToList();

            if (model.Empno != null)
                stockAdjustmentData = stockAdjustmentData.Where(s => s.Empno == model.Empno).ToList();

            if (!string.IsNullOrEmpty(model.Barcode))
                stockAdjustmentData = stockAdjustmentData.Where(s => s.Barcode == model.Barcode).ToList();

            if (!string.IsNullOrEmpty(model.SKU))
                stockAdjustmentData = stockAdjustmentData.Where(s => s.Master.SKU == model.SKU).ToList();

            if (!string.IsNullOrEmpty(model.Description))
                stockAdjustmentData = stockAdjustmentData.Where(s => s.Master.Description == model.Description).ToList();

            if (!string.IsNullOrEmpty(model.Term))
                stockAdjustmentData = stockAdjustmentData.Where(s => s.Term == model.Term).ToList();

            if (model.Shelf != null)
                stockAdjustmentData = stockAdjustmentData.Where(s => s.Shelf == model.Shelf).ToList();

            if (model.Tag != null)
                stockAdjustmentData = stockAdjustmentData.Where(s => s.Tag == model.Tag).ToList();

            if (!string.IsNullOrEmpty(model.RetailPrice))
                stockAdjustmentData = stockAdjustmentData.Where(s => s.Master.RetailPrice == model.RetailPrice).ToList();

            if (model.Quantity != null)
                stockAdjustmentData = stockAdjustmentData.Where(s => s.Quantity.ToString().Contains(model.Quantity.ToString())).ToList();


            if (!string.IsNullOrEmpty(model.searchtext))
                stockAdjustmentData = stockAdjustmentData.Where(s => s.Quantity.ToString().Contains(model.searchtext.ToString())
                || s.Shelf.ToString().Contains(model.searchtext.ToString())
                || s.Tag.ToString().Contains(model.searchtext.ToString())
                || s.Term.ToString().Contains(model.searchtext.ToString())
                || s.Master.Description.ToString().Contains(model.searchtext.ToString())
                || s.Master.RetailPrice.ToString().Contains(model.searchtext.ToString())
                || s.SKU.ToString().Contains(model.searchtext.ToString())
                || s.Department.ToString().Contains(model.searchtext.ToString())
                || s.Dload.ToString().Contains(model.searchtext.ToString())
                || s.Empno.ToString().Contains(model.searchtext.ToString())
                || s.Barcode.ToString().Contains(model.searchtext.ToString())
                ).ToList();
            return stockAdjustmentData;
        }
    }
}
