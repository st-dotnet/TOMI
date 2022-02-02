using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TOMI.Data.Database;
using TOMI.Data.Database.Entities;
using TOMI.Services.Interfaces;
using TOMI.Services.Models;

namespace TOMI.Services.Repository
{
    public class InfoLoadRepository: IInfoLoadService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<InfoLoadRepository> _logger;
        private readonly TOMIDataContext _context;
        public InfoLoadRepository(ILogger<InfoLoadRepository> logger, TOMIDataContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<InfoLoad> DeleteInfoLoad(int id)
        {
            var infoLoad= await _context.InfoLoad.FirstOrDefaultAsync(x => x.Id == id);
            if(infoLoad!=null)
            {
                _context.InfoLoad.Remove(infoLoad);
                await _context.SaveChangesAsync();
                return infoLoad;
            }
            throw new System.ComponentModel.DataAnnotations.ValidationException("Id not found!");
        }

        public async Task<InfoLoad> GetInfoLoadAsync(int id)
        {
            return await _context.InfoLoad.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<InfoLoad>> GetInfoLoadListAsync()
        {
            return await _context.InfoLoad.ToListAsync();
        }

        public async Task<InfoLoadResponse> SaveInfoLoad(InfoLoadModel model)
        {
            InfoLoad infoLoad = await _context.InfoLoad.FirstOrDefaultAsync(c => c.Id == model.Id);
            var infoload = _mapper.Map<InfoLoad>(model);
            if (infoLoad == null)
            {
                InfoLoad result = _context.InfoLoad.Add(infoload).Entity;
                await _context.SaveChangesAsync();
                return new InfoLoadResponse { InfoLoad = result, Success = true };
            }
            else
            {
                var res = _mapper.Map<InfoLoad>(model);
                _context.InfoLoad.Update(res);
                await _context.SaveChangesAsync();
                return new InfoLoadResponse { InfoLoad = res, Success = true };
            }
            throw new System.ComponentModel.DataAnnotations.ValidationException("Id not found!");
        }

        public async Task<FileInfoDataResponse> InfoData(FilterInfoDataModel infodata)
        {
            bool isSaveSuccess = false;
            string fileName;
            List<InfoDataResponse> records = new List<InfoDataResponse>();

            try
            {
                var extension = "." + infodata.File.FileName.Split('.')[infodata.File.FileName.Split('.').Length - 1];
                fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.

                var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");

                if (!Directory.Exists(pathBuilt))
                {
                    Directory.CreateDirectory(pathBuilt);
                }

                var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files",
                   fileName);


                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await infodata.File.CopyToAsync(stream);
                }
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false,
                    BadDataFound = null,
                    Delimiter = ",",
                };

                using (var reader = new StreamReader(path)) 
                using (var csv = new CsvReader(reader, config))
                {

                    records = csv.GetRecords<InfoDataResponse>().ToList();

                    var infodetails = _mapper.Map<List<InfoLoad>>(records);

                    foreach (var item in infodetails)
                    {
                        _context.InfoLoad.Add(item);
                    }
                }
                // Submit the change to the database.
                try
                {
                    await _context.SaveChangesAsync();
                    File.Delete(path);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Make some adjustments.
                    // ...
                    // Try again.
                    await _context.SaveChangesAsync();
                }
                isSaveSuccess = true;
            }
            catch (Exception e)
            {
                //log error
            }

            return new FileInfoDataResponse
            {
                InfoDataRecordCount = records.Count.ToString(),
                Success = isSaveSuccess
            }; ;
        }
    }
}
