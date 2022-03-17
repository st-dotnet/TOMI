﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;
using TOMI.Services.Models;

namespace TOMI.Services.Interfaces
{
    public  interface IInfomrationLoadingService
    {
        Task<List<spInformationLoading>> GenerateTerminalSummary();
        Task<List<spInformationTransmissionDetails>> GetInformationTransmissionDetails();

        Task<List<spOriginalTag>> GetInformationforOriginalTag(int tag);
        Task<List<spTerminalForOriginalDetials>> GetInformationdetails(int tag, string empnumber, string terminal);
        Task<Terminal_Smf> DeleteOriginalRecord(OriginalRecordModel model);
    }
}
