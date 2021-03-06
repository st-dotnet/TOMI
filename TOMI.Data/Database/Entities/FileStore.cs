using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Data.Database.Entities
{
    public class FileStore : EntityBase
    {
        public int Id { get; set; }
        public string Header { get; set; }
        public string FileName { get; set; }
        public string FileDate { get; set; }
        public string StoreNumber { get; set; }
        public string Category { get; set; }
        public string RecordCount { get; set; }
        public string Status { get; set; }
    }
}
