using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            var existingRanges= await _context.Ranges.FirstOrDefaultAsync(x => x.Id == id);
            if (existingRanges != null)
            {
                Ranges result = _context.Ranges.Remove(existingRanges).Entity;
                 _context.SaveChanges();
                return result;
            }
            throw new ValidationException("Range not found!");
        }
        public async Task<Ranges> GetRange(Guid id)
        {
            return await _context.Ranges.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<Ranges>> GetRangesAsync()
        {
            return await _context.Ranges.Include(x=>x.Group).ToListAsync(); 
        }
        public async Task<Ranges> SaveRanges(RangesModel Range)
        {
            Ranges existingRanges = await _context.Ranges.FirstOrDefaultAsync(c => c.Name == Range.Name);

            var ranges = _mapper.Map<Ranges>(Range);
            if (existingRanges == null)
            {
                Ranges result = _context.Ranges.Add(ranges).Entity;
                await _context.SaveChangesAsync();
                return result;
            }
            throw new ValidationException("Range not found!");
        }


   
    }
}
