using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            string StoredProc = "exec spgetInformationforOriginalTag " + "@Tag = " + tag + "";
            return await _context.spOriginalTag.FromSqlRaw(StoredProc).ToListAsync();
        }
        public async Task<List<spTerminalForOriginalDetials>> GetInformationfirstsectiondetails(string tag, string empNumber)
        {
            string StoredProc = "exec spgetTerminalForOriginalDetialsFirstoption " + "@Tag = " + tag + "," + "@empNumber= '" + empNumber + "'";
            return await _context.spTerminalForOriginalDetials.FromSqlRaw(StoredProc).ToListAsync();
        }
        public async Task<List<spTerminalForOriginalDetials>> GetInformationsecondsectiondetails(string tag, string empNumber)
        {
            string StoredProc = "exec spgetTerminalForOriginalDetialsSecondoption " + "@Tag = " + tag + "," + "@empNumber= '" + empNumber + "'";
            return await _context.spTerminalForOriginalDetials.FromSqlRaw(StoredProc).ToListAsync();
        }
        
        public async Task<Terminal_Smf> DeleteOriginalRecord(int tag, string empNumber,string terminal)
        {
            try
            {
                var existingRanges = await _context.Terminal_Smf.FirstOrDefaultAsync(x => x.tag == tag && x.Employee_Number==empNumber && x.Terminal== terminal);
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
        public async Task<Terminal_SmfResponse> ReNumberTagOption(UpdateTag models)
        {
            try
            {
                var existingTags = await _context.Terminal_Smf.FirstOrDefaultAsync(x => x.tag == models.Tag && x.Employee_Number == models.EmpNumber && x.Terminal == models.Terminal);
                if (existingTags.tag== models.NewTag)
                {
                    //  throw new Exception("Tag number already exit.");
                    return new Terminal_SmfResponse { Error = "Tag Number already in used! ", Success = false };
                }
                existingTags.tag = models.NewTag;
                _context.SaveChanges();
                return new Terminal_SmfResponse { terminal_smf = existingTags, Success = true };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<MergeWithNewResponse> MergeNewithOriginal(MergeWithNewInfloarding models)
        {
            try
            {
                var existingOriginalMerge  = await _context.Terminal_Smf.FirstOrDefaultAsync(x => x.tag == models.Tag && x.Employee_Number == models.EmpNumberOriginal && x.Terminal == models.TerminalOriginal);
                var existingNewMerge = await _context.Terminal_Smf.FirstOrDefaultAsync(x => x.tag == models.Tag && x.Employee_Number == models.EmpNumber && x.Terminal == models.Terminal);
                if (existingOriginalMerge.Employee_Number==null)
                {
                    return new MergeWithNewResponse { Error = "Employee number not exist in the database! ", Success = false };
                }
                existingOriginalMerge.Employee_Number = existingNewMerge.Employee_Number;
                existingOriginalMerge.Terminal = existingNewMerge.Terminal;
                existingOriginalMerge.Code = existingNewMerge.Code;
                existingOriginalMerge.total_counted = existingNewMerge.total_counted;
                existingOriginalMerge.shelf = existingNewMerge.shelf;
                existingNewMerge.Isdeleted = true;
                _context.SaveChanges();
                return new MergeWithNewResponse { terminal_smf = existingOriginalMerge, Success = true };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}

