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

        [HttpPost("ImportSalesFile")]
        public async Task<IActionResult> UploadFile([FromForm] FilterDataModel stockModel )
        {
            try
            {
                if (stockModel != null)
                    return Ok(await _storeService.SalesData(stockModel));
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
        public async Task<IActionResult> MasterFile([FromForm] FilterDataModel masterData)
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

        [HttpPost("ImportStockFile")]
        public async Task<IActionResult> StockFile([FromForm] FilterDataModel stockData)
        {
            try
            {
                if (stockData != null)
                    return Ok(await _storeService.StocksData(stockData));
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
        ///  this is used for get Sales data by customerid, storeId and date
        /// </summary>
        /// <param name="request">SalesModelRequest</param>
        /// <returns></returns>
        [HttpPost("GetSalesData")]
        public async Task<IActionResult> GetSalesData(FilterDataRequest request)
        {
            try
            {
                return Ok(await _storeService.GetSalesData(request));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Invalid Id" });
                throw;
            }
        }

        /// <summary>
        ///  this is used for get Master data by customerid, storeId and date
        /// </summary>
        /// <param name="request">MasterModelRequest</param>
        /// <returns></returns>
        [HttpPost("GetMasterData")]
        public async Task<IActionResult> GetMasterData(FilterDataRequest request)
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

        /// <summary>
        ///  this is used for get Stocks data by customerid, storeId and date
        /// </summary>
        /// <param name="request">StocksModelRequest</param>
        /// <returns></returns>
        [HttpPost("GetStocksData")]
        public async Task<IActionResult> GetStocksData(FilterDataRequest request)
        {
            try
            {
                return Ok(await _storeService.GetStocksData(request));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Invalid Id" });
                throw;
            }
        }


    }
}
