using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TOMI.Data.Database.Entities;
using TOMI.Services.Models;

namespace TOMI.Services.Interfaces
{
   public interface IProgramTerminalService
    {
        // Task<TerminalResponse> GenerateMF1(TermModel model);
        Task<List<spGenerateGenerateMF1>> GenerateMF1(int operation, int inventoryKey);
        Task<JsonResult> GetMFData(GetTerminalModel terminal);
        Task<TerminalDataModelsResponse> PostTerminal(TerminalDataModels post);
        Task<EmpDataResponse> AddEmployeeData();
    }
}
