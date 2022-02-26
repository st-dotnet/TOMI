using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Web.Http.Results;
using Microsoft.AspNetCore.Mvc;
using TOMI.Data.Database.Entities;
using TOMI.Services.Models;

namespace TOMI.Services.Interfaces
{
   public interface IProgramTerminalService
    {
        Task<TerminalResponse> GenerateMF1(TerminalModel model);
        Task<JsonResult> GetMFData(GetTerminalModel terminal);
        //Task<List<TerminalPost>> PostTerminal(TerminalPost post);
        //Task<JsonResult> PostTerminal(TerminalDataModels post);
        Task<TerminalDataModels> PostTerminal(TerminalDataModels post);
        Task<EmpDataResponse> AddEmployeeData();
    }
}
