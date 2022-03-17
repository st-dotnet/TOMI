using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
           // return await _context.spInformationLoading.FromSqlRaw("EXECUTE dbo.spMaketableQueryForTerminalSMF").ToListAsync();
            return await _context.spInformationLoading.FromSqlRaw("EXECUTE dbo.spgetInformationLoading").ToListAsync();
       
        }
        public async Task<List<spInformationTransmissionDetails>> GetInformationTransmissionDetails()
        {

            return await _context.spInformationTransmissionDetails.FromSqlRaw("EXECUTE dbo.spGetTerminalDetails").ToListAsync();
        
        }
        public async Task<List<spOriginalTag>> GetInformationforOriginalTag(int tag)
        {
            string StoredProc = "exec spgetInformationforOriginalTag " + "@Tag = " + tag +""; 
            return await _context.spOriginalTag.FromSqlRaw(StoredProc).ToListAsync(); 

        }

        public async Task<List<spTerminalForOriginalDetials>> GetInformationdetails(int tag, string empnumber, string terminal)
        {
            string StoredProc = "exec spgetTerminalForOriginalDetials " +
                "@employee = '" + empnumber + "'," + " @tag = " + tag + ", " + "@terminal = '" + terminal + "'";

            return await _context.spTerminalForOriginalDetials.FromSqlRaw(StoredProc).ToListAsync();

        }
        public async Task<Terminal_Smf> DeleteOriginalRecord(OriginalRecordModel model)
        {
            try
            {
                var existingRanges = await _context.Terminal_Smf.FirstOrDefaultAsync(x => x.tag == model.tag && x.shelf == model.shelf && x.Employee_Number == model.Employee_Number && x.Terminal == model.Terminal);
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

    }
}
