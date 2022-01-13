using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TOMI.Services.Interfaces;
using TOMI.Services.Models;

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

        [HttpPost("ImportStockFile")]
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

        [HttpPost("ImportMasterFile")]
        public async Task<IActionResult> MasterFile([FromForm] MasterDataModel masterData)
        {
            try
            {
                if (masterData != null)
                    return Ok(await _storeService.MasterData(masterData));
                else
                    return BadRequest(new { message = "please at least upload one file " });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Invalid file extension" });
                throw;
            }
        }


        /// <summary>
        ///  this is used for get stock data by customerid, storeId and date
        /// </summary>
        /// <param name="request">StockModelRequest</param>
        /// <returns></returns>
        [HttpPost("GetStockData")]
        public async Task<IActionResult> GetStockData(StockModelRequest request)
        {
            try
            {
                return Ok(await _storeService.GetStockData(request));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Invalid Id" });
                throw;
            }
        }

        /// <summary>
        ///  this is used for get stock data by customerid, storeId and date
        /// </summary>
        /// <param name="request">StockModelRequest</param>
        /// <returns></returns>
        [HttpPost("GetMasterData")]
        public async Task<IActionResult> GetMasterData(MasterModelRequest request)
        {
            try
            {
                return Ok(await _storeService.GetMasterData(request));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Invalid Id" });
                throw;
            }
        }
    }
}
