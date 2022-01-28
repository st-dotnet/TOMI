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
    public class InfoLoadRepository: IInfoLoadService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<InfoLoadRepository> _logger;
        private readonly TOMIDataContext _context;
        public InfoLoadRepository(ILogger<InfoLoadRepository> logger, TOMIDataContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<InfoLoad> DeleteInfoLoad(int id)
        {
            var infoLoad= await _context.InfoLoad.FirstOrDefaultAsync(x => x.Id == id);
            if(infoLoad!=null)
            {
                _context.InfoLoad.Remove(infoLoad);
                await _context.SaveChangesAsync();
                return infoLoad;
            }
            throw new ValidationException("Id not found!");
        }

        public async Task<InfoLoad> GetInfoLoadAsync(int id)
        {
            return await _context.InfoLoad.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<InfoLoad>> GetInfoLoadListAsync()
        {
            return await _context.InfoLoad.ToListAsync();
        }

        public async Task<InfoLoadResponse> SaveInfoLoad(InfoLoadModel model)
        {
            InfoLoad infoLoad = await _context.InfoLoad.FirstOrDefaultAsync(c => c.Id == model.Id);
            var infoload = _mapper.Map<InfoLoad>(model);
            if (infoLoad == null)
            {
                InfoLoad result = _context.InfoLoad.Add(infoload).Entity;
                await _context.SaveChangesAsync();
                return new InfoLoadResponse { InfoLoad = result, Success = true };
            }
            else
            {
                var res = _mapper.Map<InfoLoad>(model);
                _context.InfoLoad.Update(res);
                await _context.SaveChangesAsync();
                return new InfoLoadResponse { InfoLoad = res, Success = true };
            }
            throw new ValidationException("Id not found!");
        }
    }
}
