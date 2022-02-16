using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TOMI.Data.Database;
using TOMI.Data.Database.Entities;
using TOMI.Services.Interfaces;

namespace TOMI.Services.Repository
{

    public class FileStoreRepository : IFileStoreService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<FileStoreRepository> _logger;
        private readonly TOMIDataContext _context;

        public FileStoreRepository(ILogger<FileStoreRepository> logger, TOMIDataContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }
        public async Task<FileStore> GetFileStroreAsync(string store, string date)
        {
            //string StockDate = date;
            //string ActStockDate = String.Format("{0:MMyy}", StockDate);
            return await _context.FileStore.FirstOrDefaultAsync(x => x.StoreNumber==store && x.FileDate== date && x.Category== "OrderJob");
        }
    }
}
