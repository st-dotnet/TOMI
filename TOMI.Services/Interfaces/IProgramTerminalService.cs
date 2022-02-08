﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;
using TOMI.Services.Models;

namespace TOMI.Services.Interfaces
{
   public interface IProgramTerminalService
    {
        Task<MF1> GenerateMasterFiles(TerminalModel model);
    }
}
