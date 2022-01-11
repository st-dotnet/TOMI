using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using TOMI.Services.Interfaces;
using TOMI.Services.Models;
using TOMI.Web.Models;

namespace TOMI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private IStoreService _storeService;
        private IMapper _mapper;
        public StoreController(IStoreService storeService, IMapper mapper)
        {
            _storeService = storeService;
            _mapper = mapper;
        }
        [HttpPost]
        [Route("AddStore")]
        public async Task<IActionResult> AddStoreAsync(StoreModel store)
        {
            try
            {
                return Ok(await _storeService.CreateStore(store));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        [HttpGet("GetStore/{customerId}")]
        public async Task<IActionResult> GetUsersByIdAsync(string customerId)
        {
            try
            {
                return Ok(await _storeService.GetUserByCustomereAsync(customerId));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] StockModel stockModel )
        {
            try
            {
                if (stockModel != null)
                    return Ok(await _storeService.StocksData(stockModel));
                else
                    return BadRequest(new { message = "please at least upload one file " });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Invalid file extension" });
                throw;
            }  
        }
    }
}
