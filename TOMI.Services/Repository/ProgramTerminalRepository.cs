using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TOMI.Data.Database;
using TOMI.Data.Database.Entities;
using TOMI.Services.Interfaces;
using TOMI.Services.Models;

namespace TOMI.Services.Repository
{
  public class ProgramTerminalRepository: IProgramTerminalService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ProgramTerminalRepository> _logger;
        private readonly TOMIDataContext _context;
        public ProgramTerminalRepository(ILogger<ProgramTerminalRepository> logger, TOMIDataContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<TerminalResponse> GenerateMF1(TerminalModel model)
        {
            List<MF1> mf1 = new();
            List<MF2> mf2 = new();
            var masterdata = _context.OrderJob.Where(x => x.CustomerId == model.CustomerId && x.StoreId == model.StoreId && x.StockDate == model.Date).ToList();
            foreach (var item in masterdata)
            {
                MF2 data2 = new MF2();
                MF1 data = new MF1();
                data.Code = item.Code;
                data.CustomerId = (Guid)model.CustomerId;
                data.Department = item.Department;
                data.inventory_key = model.InventaryKey;
                data.counted_status = true;
                data.Employee_Number = "432434234";
                data.count_time = DateTime.Now;
                data.Terminal = "ds";
                data.StoreId = (Guid)model.StoreId;
                data.Inventory_Date = DateTime.Now;
                data.Sale_Price = 0;
                data.tag = 1;
                data.shelf = 1;
                data.operation = 1;
                data.creation_time = DateTime.Now;
                data.inventory_key = model.InventaryKey;
                data.sync_to_terminal_status = false;
                data.sync_to_terminal_time = DateTime.Now;
                data.sync_back_from_terminal_status = false;
                data.sync_back_from_terminal_time = DateTime.Now;
                data.count_type = model.CountType;
                data.total_counted = 1;
                data.count_time = DateTime.Now;
                data.nof = false;
                data.counted_status = false;
                //insert data in MF2
                data2.Department = item.Department;
                data2.creation_time = (DateTimeOffset)item.StockDate;

                mf1.Add(data);
                mf2.Add(data2);

            }
            await _context.BulkInsertAsync(mf1);
            await _context.BulkInsertAsync(mf2);
            return new TerminalResponse { Success = true };
        }

       
    }
}
