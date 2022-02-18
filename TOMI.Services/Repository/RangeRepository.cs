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
            try
            {
                var existingRanges = await _context.Ranges.FirstOrDefaultAsync(x => x.Id == id);

                if (existingRanges != null)
                {
                    var rangesexist = await _context.StockAdjustment.FirstOrDefaultAsync(x => x.Id == id);
                    if (rangesexist != null)
                    {
                        throw new ValidationException("Tag ranges already in used!");
                    }

                    _context.Ranges.Remove(existingRanges);
                    await _context.SaveChangesAsync();
                    return existingRanges;
                }
                throw new ValidationException("Range not found!");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        public async Task<Ranges> GetRange(Guid id)
        {
            try
            {
                return await _context.Ranges.OrderBy(x => x.TagFrom).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<List<Ranges>> GetRangesAsync()
        {
            try
            {
                return await _context.Ranges.Include(x => x.Group).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<RangesModel> SaveRanges(RangesModel rangeModel)
        {
            try
            {
                Ranges existingRanges = await _context.Ranges.FirstOrDefaultAsync(c => c.Id == rangeModel.Id);

                var ranges = _mapper.Map<Ranges>(rangeModel);

                if (existingRanges == null)
                {
                    Ranges result = _context.Ranges.Add(ranges).Entity;
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
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        public async Task<Ranges> GetLastRange(Guid id)
        {
            try
            {
                return await _context.Ranges
            .Where(m => m.GroupId.Equals(id))
            .OrderByDescending(m => m.Id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        //public  int GetMinMaxRange()
        //{
        //    int MaxRangeValue = _context.Ranges.MaxAsync(s =>Convert.ToInt32(s.TagTo));

        //    return MaxRangeValue;
        //}
        public async Task<int> GetMinMaxRange()
        {
            try
            {
                var maxRange = await _context.Ranges.MaxAsync(x => x.TagTo);
                if (maxRange.ToString() != null)
                {
                    return Convert.ToInt32(maxRange) + 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<bool> GetTag(int tag)
        {
            try
            {
                var findtag = await _context.Ranges.FirstOrDefaultAsync(x => x.TagFrom <= tag && x.TagTo >= tag);
                if (findtag != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}

