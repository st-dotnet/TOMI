using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.AspNetCore.Mvc;
using TOMI.Data.Database.Entities;
using TOMI.Services.Models;

namespace TOMI.Services.Interfaces
{
   public interface IProgramTerminalService
    {
        Task<TerminalResponse> GenerateMF1(TerminalModel model);
        //Task<TerminalMF2Response> GenerateMF2(MF2Model model);
        Task<JsonResult> GetMFData(TerminalModel terminal);
       // Task<TerminalResponse> GetMFData(TerminalModel terminal);
    }
}
