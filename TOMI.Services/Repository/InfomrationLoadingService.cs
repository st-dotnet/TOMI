using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TOMI.Data.Database;
using TOMI.Data.Database.Entities;
using TOMI.Services.Interfaces;
using TOMI.Services.Models;

namespace TOMI.Services.Repository
{
    public class InfomrationLoadingService : IInfomrationLoadingService
    {
        private readonly TOMIDataContext _context;
        public InfomrationLoadingService(TOMIDataContext context)
        {
           _context = context;
        }
        public async Task<List<spInformationLoading>> GenerateTerminalSummary()
        {
           return await _context.spInformationLoading.FromSqlRaw("EXECUTE dbo.spgetInformationLoading").ToListAsync();
        }
        public async Task<List<spInformationTransmissionDetails>> GetInformationTransmissionDetails()
        {

            return await _context.spInformationTransmissionDetails.FromSqlRaw("EXECUTE dbo.spGetTerminalDetails").ToListAsync();
        
        }
    }
}
