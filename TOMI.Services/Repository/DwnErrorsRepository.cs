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
    public class DwnErrorsRepository : IDwnErrorsService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DwnErrorsRepository> _logger;
        private readonly TOMIDataContext _context;
        public DwnErrorsRepository(ILogger<DwnErrorsRepository> logger, TOMIDataContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }
        public async Task<DwnErrors> DeleteDwnErrors(int Id)
        {
            var dwnerror = await _context.DwnErrors.FirstOrDefaultAsync(x => x.id == Id);
            if (dwnerror != null)
            {
                _context.DwnErrors.Remove(dwnerror);
                await _context.SaveChangesAsync();
                return dwnerror;
            }
            throw new ValidationException("Id not found!");
        }

        public async Task<DwnErrors> GetDwnErrorsAsync(int id)
        {
            return await _context.DwnErrors.FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<List<DwnErrors>> GetDwnErrorsListAsync()
        {
            return await _context.DwnErrors.ToListAsync();
        }

        public async Task<DwnErrorsResponse> SaveDwnErrors(DwnErrorsModel model)
        {
            DwnErrors dwnError = await _context.DwnErrors.FirstOrDefaultAsync(c => c.id == model.id);
            var dwnErrors = _mapper.Map<DwnErrors>(model);
            if (dwnError == null)
            {
                DwnErrors result = _context.DwnErrors.Add(dwnErrors).Entity;
                await _context.SaveChangesAsync();
                return new DwnErrorsResponse { dwnErrors = result, Success = true };
            }
            else
            {
                var res = _mapper.Map<DwnErrors>(model);
                _context.DwnErrors.Update(res);
                await _context.SaveChangesAsync();
                return new DwnErrorsResponse { dwnErrors = res, Success = true };
            }
            throw new ValidationException("Id not found!");
        }
    }
}
