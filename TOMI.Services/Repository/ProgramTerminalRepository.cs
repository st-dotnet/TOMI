using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
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

        public async Task<TerminalResponse> GenerateMF1(TermModel model)
        {
            try
            {
                List<Terminal_Smf> mf1 = new();
                List<Terminal_Smf> updatemf1 = new();
                List<Terminal_Department> mf2 = new();

                var masterdata = await _context.OrderJob.ToListAsync();

                var existingData = from a in _context.Terminal_Smf from b in _context.OrderJob where a.Code == b.Code && a.Department == b.Department select a;

                foreach (var item in masterdata)
                {
                    var data1 = existingData.FirstOrDefault(x => x.Code == item.Code && x.Department == item.Department);
                    if (data1 != null)
                    {
                        data1.Code = item.Code;
                        data1.Department = item.Department;
                        data1.Inventory_Date = item.StockDate;
                        data1.Sale_Price = Convert.ToDecimal(item.SalePrice);
                        data1.operation = model.Operation;
                        data1.inventory_key = model.InventoryKey;
                        updatemf1.Add(data1);
                    }
                    else
                    {
                        Terminal_Smf data = new()
                        {
                            Code = item.Code,
                            Department = item.Department,
                            Inventory_Date = item.StockDate,
                            Sale_Price = Convert.ToDecimal(item.SalePrice),
                            operation = model.Operation,
                            creation_time = DateTime.Now,
                            sync_to_terminal_status = false,
                            sync_to_terminal_time = DateTime.Now,
                            sync_back_from_terminal_status = false,
                            sync_back_from_terminal_time = DateTime.Now,
                            count_time = DateTime.Now,
                            counted_status = false,
                            CustomerId = (Guid)item.CustomerId,
                            StoreId = (Guid)item.StoreId,
                            Id = new Guid(),
                            inventory_key = model.InventoryKey
                        };

                        mf1.Add(data);
                    }

                    Terminal_Department data2 = new Terminal_Department();
                    Terminal_Department checkMF = new Terminal_Department();
                    data2.Id = new Guid();
                    data2.Department = item.Department;
                    data2.creation_time = DateTime.Now;
                    var exist = _context.Terminal_Department.FirstOrDefault(x => x.Department == item.Department);
                    if (exist == null)
                    {
                        checkMF = mf2.FirstOrDefault(x => x.Department == item.Department);
                    }
                    if (checkMF == null && exist == null)
                    {
                        mf2.Add(data2);
                    }


                }
                if (mf2 != null)
                    await _context.BulkInsertAsync(mf2);

                if (updatemf1 != null)
                    await _context.BulkUpdateAsync(updatemf1);

                if (mf1 != null)
                    await _context.BulkInsertAsync(mf1);
                return new TerminalResponse { Success = true };


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

                mF1AndEmp.TerminalSmf = await (from a in _context.Terminal_Smf
                                               join c in _context.Terminal_Department on a.Department equals c.Department
                                               join b in _context.OrderJob on c.Department equals b.Department
                                               select new TerminalSmf
                                               {
                                                   Code = a.Code,
                                                   CountedStatus = a.counted_status,
                                                   Department = a.MF2.Department,
                                                   Customer = a.CustomerId,
                                                   Terminal = a.Terminal,
                                                   Store = a.Store.Name,
                                                   EmployeeNumber = a.Employee_Number,
                                                   InventoryDate = (DateTimeOffset)a.Inventory_Date,
                                                   SalePrice = a.Sale_Price,
                                                   Tag = a.tag.ToString(),
                                                   Shelf = a.shelf.ToString(),
                                                   Operation = terminal.operation,
                                                   InventoryKey = a.inventory_key,
                                                   CountType = a.count_type,
                                                   TotalCounted = a.total_counted,
                                                   CountTime = a.count_time,
                                                   Nof = a.nof,
                                                   Description = b.Description,

                                               })
                                            .ToListAsync();

                mF1AndEmp.Employees = await (from a in _context.Employee
                                             select new Empdata
                                             {
                                                 EmpNumber = a.EmpNumber,
                                                 EmpName = a.EmpName,
                                                 LastName = a.LastName
                                             })
                .ToListAsync();

                var terminaldata = _context.Terminal_Smf.Include(c => c.MF2)
                    .FirstOrDefault(x => x.operation == terminal.operation && x.inventory_key == x.inventory_key);
                mF1AndEmp.CustomerId = terminaldata.CustomerId;
                mF1AndEmp.StoreId = terminaldata.StoreId;

                return new JsonResult(mF1AndEmp);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<TerminalDataModelsResponse> PostTerminal(TerminalDataModels post)
        {
            try
            {
                List<Terminal_Smf> mf1 = new();
                List<Terminal_Smf> updatEmf1 = new();

                var existingRecords = await _context.Terminal_Smf.Where(x => x.tag.ToString() == post.Tag).ToListAsync();

                var posts = post.Shelves;
                foreach (var item in posts)
                {
                    var checkexistingRecords = existingRecords.FirstOrDefault(x => x.shelf.ToString() == item.Shelf);
                    if (checkexistingRecords != null)
                    {
                        checkexistingRecords.tag = Convert.ToInt32(post.Tag);
                        checkexistingRecords.shelf = Convert.ToInt32(item.Shelf);
                        foreach (var item1 in item.Products)
                        {
                            checkexistingRecords.Code = item1.Code;
                            checkexistingRecords.Department = item1.Department;
                            checkexistingRecords.total_counted = item1.TotalCounted;
                            checkexistingRecords.Inventory_Date = item1.InventoryDate;
                            updatEmf1.Add(checkexistingRecords);
                        }
                    }
                    else
                    {

                        foreach (var item1 in item.Products)
                        {
                            Terminal_Smf mF = new()
                            {
                                Id = new Guid(),
                                tag = Convert.ToInt32(post.Tag),
                                CustomerId = post.CustomerId,
                                StoreId = post.StoreId,
                                shelf = Convert.ToInt32(item.Shelf),
                                Code = item1.Code,
                                Department = item1.Department,
                                total_counted = item1.TotalCounted,
                                Inventory_Date = item1.InventoryDate,
                            };
                            mf1.Add(mF);
                        }
                    }

                    if (updatEmf1 != null)
                        await _context.BulkUpdateAsync(updatEmf1);

                    if (mf1 != null)
                        await _context.BulkInsertAsync(mf1);

                }

                return new TerminalDataModelsResponse { Success = true, Message = "Data Saved Successfully", Status = HttpStatusCode.OK };
            }

            catch (Exception ex)
            {
                return new TerminalDataModelsResponse { Success = false, Message = ex.ToString(), Status = HttpStatusCode.InternalServerError };
            }
        }
    }
}












