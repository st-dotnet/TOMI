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
            try
            {
                StockAdjustmentlog stockAdjustmentlog = new StockAdjustmentlog();
                StockAdjustment stockAdjustment = await _context.StockAdjustment.FirstOrDefaultAsync(c => c.Id == model.Id);
                int record = 1;
                if (stockAdjustment == null)
                {
                    var maxRange = await _context.StockAdjustment.MaxAsync(x => x.Rec);
                    if (maxRange.ToString() != null)
                    {
                        record = Convert.ToInt32(maxRange) + 1;
                    }
                    model.Rec = record;
                    var stockadjustment = _mapper.Map<StockAdjustment>(model);
                    StockAdjustment result = _context.StockAdjustment.Add(stockadjustment).Entity;
                    await _context.SaveChangesAsync();
                    return new StockAdjustmentResponse { Adjustment = result, Success = true };
                }
                else
                {
                    //Log
                    stockAdjustmentlog.Term = model.Term;
                    stockAdjustmentlog.Rec = model.Rec;
                    stockAdjustmentlog.Dload = model.Dload;
                    stockAdjustmentlog.Tag = model.Tag;
                    stockAdjustmentlog.Shelf = model.Shelf;
                    stockAdjustmentlog.Barcode = model.Barcode;
                    stockAdjustmentlog.Quantity = model.Quantity;
                    StockAdjustmentlog resultlog = _context.StockAdjustmentlog.Add(stockAdjustmentlog).Entity;
                    await _context.SaveChangesAsync();

                    //var res = _mapper.Map<StockAdjustment>(model);
                    stockAdjustment.Term = model.Term;
                    stockAdjustment.Rec = model.Rec;
                    stockAdjustment.Dload = model.Dload;
                    stockAdjustment.Tag = model.Tag;
                    stockAdjustment.Shelf = model.Shelf;
                    stockAdjustment.Barcode = model.Barcode;
                    stockAdjustment.Quantity = model.Quantity;
                    stockAdjustmentlog.Status = "Edit";
                    _context.StockAdjustment.Update(stockAdjustment);
                    await _context.SaveChangesAsync();
                    return new StockAdjustmentResponse { Adjustment = stockAdjustment, Success = true };
                }
                throw new ValidationException("Data not found!");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<StockAdjustment> DeleteStockAdjustment(Guid id)
        {
            try
            {
                var existingRanges = await _context.StockAdjustment.FirstOrDefaultAsync(x => x.Id == id);
                if (existingRanges.Isdeleted == false)
                {
                    existingRanges.Isdeleted = true;

                    _context.SaveChanges();
                }
                return existingRanges;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }

        public async Task<StockAdjustment> GetStockAdjustmentAsync(Guid id)
        {
            try
            {
                return await _context.StockAdjustment.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<List<StockAdjustment>> GetStockAdjustmentListAsync()
        {
            try
            {
                return await _context.StockAdjustment.Include(x => x.OrderJob).Where(x => !x.Isdeleted).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<List<StockAdjustment>> GoToRecord(int recId)
        {
            try
            {
                List<StockAdjustment> records = new();
                StockAdjustment recid = await _context.StockAdjustment.Include(x => x.OrderJob).FirstOrDefaultAsync(x => x.Rec == recId);
                if (recid != null)
                {
                    records.Add(recid);
                }
                return records;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<List<StockAdjustment>> GetDeletedRecord()
        {
            try
            {
                return await _context.StockAdjustment.Include(x => x.OrderJob).Where(x => x.Isdeleted).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<List<StockAdjustment>> ChangeDeletedRecStatus(Guid recid)
        {
            try
            {
                var toBeDeleted = await _context.StockAdjustment.Where(x => x.Id == recid && x.Isdeleted == true).ToListAsync();

                toBeDeleted.ForEach(a => { a.Isdeleted = false; });

                await _context.SaveChangesAsync();

                return toBeDeleted;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }



        }

        public async Task<OrderjobResponse> MasterDataBySku(string sku)
        {
            try
            {
                var stock = await _context.OrderJob.FirstOrDefaultAsync(x => x.SKU == sku);
                if (stock == null)
                {
                    return new OrderjobResponse { Error = "Sku id doesn't exist", Success = false };
                }
                else
                {
                    return new OrderjobResponse { orderJob = stock, Success = true };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<OrderResponse> MasterDataByBarCodes()
        {
            try
            {
                var stock = await _context.OrderJob.ToListAsync();
             
                    return new OrderResponse { orderJob = stock, Success = true };
               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<OrderjobResponse> MasterDataByBarCode(string barcode)
        {
            try
            {
                var stock = await _context.OrderJob.FirstOrDefaultAsync(x => x.Code == barcode);
                if (stock == null)
                {
                    return new OrderjobResponse { Error = "Barcode id doesn't exist", Success = false };
                }
                else
                {
                    return new OrderjobResponse { orderJob = stock, Success = true };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<List<StockAdjustment>> FilterStock(StockAdjustmentFilterModel model)
        {
            try
            {
                var stockAdjustmentData = await _context.StockAdjustment.Include(x => x.OrderJob).ToListAsync();

                if (!string.IsNullOrEmpty(model.Department.ToString()))
                    stockAdjustmentData = stockAdjustmentData.Where(s => s.Department == model.Department).ToList();

                if (model.Dload != null)
                    stockAdjustmentData = stockAdjustmentData.Where(s => s.Dload == model.Dload).ToList();

                if (model.Empno != null)
                    stockAdjustmentData = stockAdjustmentData.Where(s => s.Empno == model.Empno).ToList();

                if (!string.IsNullOrEmpty(model.Barcode))
                    stockAdjustmentData = stockAdjustmentData.Where(s => s.Barcode == model.Barcode).ToList();

                if (!string.IsNullOrEmpty(model.SKU))
                    stockAdjustmentData = stockAdjustmentData.Where(s => s.OrderJob.SKU == model.SKU).ToList();

                if (!string.IsNullOrEmpty(model.Description))
                    stockAdjustmentData = stockAdjustmentData.Where(s => s.OrderJob.Description == model.Description).ToList();

                if (!string.IsNullOrEmpty(model.Term))
                    stockAdjustmentData = stockAdjustmentData.Where(s => s.Term == model.Term).ToList();

                if (model.Shelf != null)
                    stockAdjustmentData = stockAdjustmentData.Where(s => s.Shelf == model.Shelf).ToList();

                if (model.Tag != null)
                    stockAdjustmentData = stockAdjustmentData.Where(s => s.Tag == model.Tag).ToList();

                if (!string.IsNullOrEmpty(model.RetailPrice))
                    stockAdjustmentData = stockAdjustmentData.Where(s => s.OrderJob.SalePrice == model.RetailPrice).ToList();

                if (model.Quantity != null)
                    stockAdjustmentData = stockAdjustmentData.Where(s => s.Quantity.ToString().Contains(model.Quantity.ToString())).ToList();


                if (!string.IsNullOrEmpty(model.searchtext))
                    stockAdjustmentData = stockAdjustmentData.Where(s => s.Quantity.ToString().Contains(model.searchtext.ToString())
                    || s.Shelf.ToString().Contains(model.searchtext.ToString())
                    || s.Tag.ToString().Contains(model.searchtext.ToString())
                    || s.Term.ToString().Contains(model.searchtext.ToString())
                    || s.OrderJob.Description.ToString().Contains(model.searchtext.ToString())
                    || s.OrderJob.SalePrice.ToString().Contains(model.searchtext.ToString())
                    || s.SKU.ToString().Contains(model.searchtext.ToString())
                    || s.Department.ToString().Contains(model.searchtext.ToString())
                    || s.Dload.ToString().Contains(model.searchtext.ToString())
                    || s.Empno.ToString().Contains(model.searchtext.ToString())
                    || s.Barcode.ToString().Contains(model.searchtext.ToString())
                    ).ToList();
                return stockAdjustmentData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<List<OrderJob>> GetMasterDataByCustomerId(Guid custid)
        {
            try
            {
                return await _context.OrderJob.Where(x => x.CustomerId == custid).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<StockAdjustmentResponse> VoidTag(int[] tag)
        {
            try
            {
                List<StockAdjustment> records = new();
                foreach (var item in tag)
                {
                    var tagdata = await _context.StockAdjustment.FirstOrDefaultAsync(x => x.Tag == item);
                    if (tagdata != null)
                    {
                        tagdata.Isdeleted = true;
                        tagdata.Quantity = 0;
                        records.Add(tagdata);
                    }
                }
                await _context.BulkUpdateAsync(records);

                return new StockAdjustmentResponse { Success = true };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<RangesResponse> VoidTagData(Guid id)
        {
            try
            {
                Ranges records = await _context.Ranges.FirstOrDefaultAsync(x => x.Id == id);
               //var tagdata = await _context.Ranges.FirstOrDefaultAsync(x => x.Id == id);
                if (records != null)
                {
                    records.IsDeleted = true;
                }
                _context.Ranges.UpdateRange(records);
                 await _context.SaveChangesAsync();
                return new RangesResponse { Success = true };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }
    }
}


               