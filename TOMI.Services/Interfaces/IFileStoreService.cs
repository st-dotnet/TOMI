using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;

namespace TOMI.Services.Interfaces
{
    public interface IFileStoreService
    {
        Task<FileStore> GetFileStroreAsync(string store,string date);
    }
}
