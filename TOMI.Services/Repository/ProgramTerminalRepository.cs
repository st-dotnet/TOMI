﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TOMI.Data.Database;
using TOMI.Data.Database.Entities;
using TOMI.Services.Interfaces;
using TOMI.Services.Models;

namespace TOMI.Services.Repository
{
    public class ProgramTerminalRepository : IProgramTerminalService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ProgramTerminalRepository> _logger;
        private readonly TOMIDataContext _context;
        private bool checkMF;

        public ProgramTerminalRepository(ILogger<ProgramTerminalRepository> logger, TOMIDataContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<TerminalResponse> GenerateMF1(TerminalModel model)
        {
            try
            {
                List<MF1> mf1 = new();
                List<MF2> mf2 = new();
                var masterdata = _context.OrderJob.Where(x => x.CustomerId == model.CustomerId && x.StoreId == model.StoreId && x.StockDate == model.Date).ToList();
                foreach (var item in masterdata)
                {
                    MF2 data2 = new MF2();
                    MF1 data = new MF1();
                    MF2 checkMF = new MF2();
                    data2.Department = item.Department;
                    data2.creation_time = DateTime.Now;
                    var exist = _context.MF2.FirstOrDefault(x => x.Department == item.Department);
                    if (exist == null)
                    {
                        checkMF = mf2.FirstOrDefault(x => x.Department == item.Department);
                    }
                    if (checkMF == null && exist == null)
                    {
                        mf2.Add(data2);
                    }
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
                    data.sync_to_terminal_status = false;
                    data.sync_to_terminal_time = DateTime.Now;
                    data.sync_back_from_terminal_status = false;
                    data.sync_back_from_terminal_time = DateTime.Now;
                    data.count_type = model.CountType;
                    data.total_counted = 1;
                    data.count_time = DateTime.Now;
                    data.nof = false;
                    data.counted_status = false;
                    mf1.Add(data);
                }
                await _context.BulkInsertAsync(mf2);



                await _context.BulkInsertAsync(mf1);



                return new TerminalResponse { Success = true };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }



        public async Task<EmpDataResponse> AddEmployeeData()
        {
            try
            {

                List<Employee> emp = new List<Employee>
                {
                new Employee {EmpNumber="4001OM", EmpName = "MIGUEL",LastName="OCAMPO HERNANDEZ",Postion="IM", inventory_key="12345" },
                new Employee { EmpNumber="4001GA", EmpName = "GREGORIO",LastName="ADAN GONZALEZ",Postion="IM", inventory_key="12345" },
                new Employee { EmpNumber="4028MM", EmpName = "MARIO",LastName="MORENO",Postion="IT", inventory_key="12345" },
                new Employee { EmpNumber="6023AP", EmpName = "ALFONSO",LastName="PRADO",Postion="AM", inventory_key="12345" }
                };

                await _context.BulkInsertAsync(emp);

                return new EmpDataResponse { Success = true };
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<JsonResult> GetMFData(GetTerminalModel terminal)
        {
            MF1AndEmp mF1AndEmp = new();
            try
            {
                mF1AndEmp.TerminalSmf = await _context.MF1.Include(c => c.MF2)
                .Where(x => x.count_type == terminal.CountType && x.inventory_key == x.inventory_key)
                .Select(x => new TerminalSmf
                {
                    Code = x.Code,
                    CountedStatus = x.counted_status,
                    Department = x.MF2.Department,
                    Customer = x.CustomerId,
                    Terminal = x.Terminal,
                    Store = x.Store.Name,
                    EmployeeNumber = x.Employee_Number,
                    InventoryDate = (DateTimeOffset)x.Inventory_Date,
                    SalePrice = x.Sale_Price,
                    Tag = x.tag,
                    Shelf = x.shelf,
                    Operation = x.operation,
                    InventoryKey = x.inventory_key,
                    CountType = x.count_type,
                    TotalCounted = x.total_counted,
                    CountTime = x.count_time,
                    Nof = x.nof
                })
                .ToListAsync();

                mF1AndEmp.Empdata = await (from a in _context.Employee
                                           select new Empdata
                                           {
                                               EmpNumber = a.EmpNumber,
                                               EmpName = a.EmpName,
                                               LastName = a.LastName
                                           })
                .ToListAsync();

                return new JsonResult(mF1AndEmp);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<TerminalDataModels> PostTerminal(TerminalDataModels post)
        {
            try
            {
                List<MF1> mf1 = new();
                MF1 existingRanges = await _context.MF1.FirstOrDefaultAsync(c => c.tag == post.Tag);
                MF1 mF = new MF1();
                var posts = post.Shelves;



                foreach (var item in posts)
                {
                    mF = new MF1();
                    mF.Id = new Guid();
                    mF.tag = post.Tag;
                    mF.shelf = (int)item.Shelf;
                    mF.CustomerId = post.CustomerId;
                    mF.StoreId = post.StoreId;
                    foreach (var item1 in item.products)
                    {
                        mF.Code = item1.Code;
                        mF.Department = item1.Department;
                        mF.total_counted = item1.total_counted;
                        mF.Inventory_Date = item1.Inventory_Date;
                        mf1.Add(mF);
                    }

                }

                await _context.BulkInsertAsync(mf1);
                // _context.SaveChangesAsync();



                return post;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


    }
}
