﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TOMI.Services.Models
{
    public class StockModel
    {
        public IFormFile File { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public string StockDate { get; set; }

    }
}
