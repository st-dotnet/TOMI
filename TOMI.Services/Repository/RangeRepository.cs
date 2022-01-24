using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TOMI.Data.Database;
using TOMI.Data.Database.Entities;
using TOMI.Services.Interfaces.RangesService;
using TOMI.Services.Models;

namespace TOMI.Services.Repository
{
    public class RangeRepository : IRangesService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<RangeRepository> _logger;
        private readonly TOMIDataContext _context;
        public RangeRepository(ILogger<RangeRepository> logger, TOMIDataContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }
        public async Task<Ranges> DeleteRange(Guid id)
        {
            var existingRanges = await _context.Ranges.FirstOrDefaultAsync(x => x.Id == id);
            if (existingRanges.IsDeleted == false)
            {
                existingRanges.IsDeleted = true;
                await _context.SaveChangesAsync();
                return existingRanges;
            }
            throw new ValidationException("Range not found!");
        }
        public async Task<Ranges> GetRange(Guid id)
        {
            return await _context.Ranges.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<Ranges>> GetRangesAsync()
        {
            return await _context.Ranges.Include(x => x.Group).ToListAsync();
        }
        public async Task<RangesModel> SaveRanges(RangesModel rangeModel)
        {
            Ranges existingRanges = await _context.Ranges.FirstOrDefaultAsync(c => c.Id == rangeModel.Id);

            var ranges = _mapper.Map<Ranges>(rangeModel);

            if (existingRanges == null)
            {
                Ranges result = _context.Ranges.Add(existingRanges).Entity;
            }
            else
            {
                //var res= _mapper.Map<Ranges>(rangeModel);
                existingRanges.Name = rangeModel.Name;
                existingRanges.TagFrom = rangeModel.TagFrom;
                existingRanges.TagTo = rangeModel.TagTo;
                existingRanges.GroupId = rangeModel.GroupId;
                _context.Ranges.Update(existingRanges);
            }


            await _context.SaveChangesAsync();
            return rangeModel;
            throw new ValidationException("Range not found!");
        }

        public async Task<Ranges> GetLastRange(Guid id)
        {
            return await _context.Ranges
            .Where(m => m.GroupId.Equals(id))
            .OrderByDescending(m => m.Id).FirstOrDefaultAsync();
        }

        //public  int GetMinMaxRange()
        //{
        //    int MaxRangeValue = _context.Ranges.MaxAsync(s =>Convert.ToInt32(s.TagTo));

        //    return MaxRangeValue;
        //}
        public async Task<int> GetMinMaxRange()
        {
            var maxRange = await _context.Ranges.MaxAsync(x => Convert.ToInt32(x.TagTo));
            if (maxRange != null)
            {
                return Convert.ToInt32(maxRange);
            }
            else
            {
                return 0;
            }
        }
    }
}

