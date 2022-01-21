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
        public async Task<StockAdjustmentModel> SaveStockAdjustment(StockAdjustmentModel model)
        {
            StockAdjustment stockAdjustment = await _context.StockAdjustment.FirstOrDefaultAsync(c => c.Id == model.Id);

            var stockadjustment = _mapper.Map<StockAdjustment>(model);

            if (stockAdjustment == null)
            {
                StockAdjustment result = _context.StockAdjustment.Add(stockadjustment).Entity;
            }
            else
            {
                var res = _mapper.Map<StockAdjustment>(model);
                _context.StockAdjustment.Update(res);
            }


            await _context.SaveChangesAsync();
            return model;
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
            return await _context.StockAdjustment.ToListAsync();
        }

        public async Task<List<StockAdjustment>> GoToRecord(Guid id)
        {
            StockAdjustment recid = await _context.StockAdjustment.FirstOrDefaultAsync(x =>x.Rec==id);
            if (recid != null)
            {
                return await _context.StockAdjustment.ToListAsync();
            }

            return null;
        }

        public async Task<List<StockAdjustment>> GetDeletedRecord(Guid recid)
        {
            return await _context.StockAdjustment.Where(x => x.Rec == recid && x.Isdeleted == true).ToListAsync();
        }

        public async Task<List<StockAdjustment>> ChangeDeletedRecStatus(Guid recid)
        {
            var toBeDeleted = await _context.StockAdjustment.Where(x => x.Rec == recid && x.Isdeleted == true).ToListAsync();

            toBeDeleted.ForEach(a => { a.Isdeleted = false; });

            await _context.SaveChangesAsync();

            return toBeDeleted;
        }
    }
}
