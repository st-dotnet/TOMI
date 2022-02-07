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

        [HttpPost("ImportDepartmentFile")]
        public async Task<IActionResult> DepartmentFile([FromForm] FilterDataModel model)
        {
            try
            {
                if (model != null)
                    return Ok(await _storeService.DepartmentsData(model));
                else
                    return BadRequest(new { message = "please at least upload one file " });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Invalid file extension" });
                throw;
            }
        }

        [HttpPost("ImportNewStockFile")]
        public async Task<IActionResult> NewStockFile([FromForm] FilterDataModel model)
        {
            try
            {
                if (model != null)
                    return Ok(await _storeService.StockData(model));
                else
                    return BadRequest(new { message = "please at least upload one file " });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Invalid file extension" });
                throw;
            }
        }

        [HttpPost("ImportReservedFile")]
        public async Task<IActionResult> ReservedFile([FromForm] FilterDataModel model)
        {
            try
            {
                if (model != null)
                    return Ok(await _storeService.ReservedData(model));
                else
                    return BadRequest(new { message = "please at least upload one file " });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Invalid file extension" });
                throw;
            }
        }


        [HttpPost("ImportCategoriesFile")]
        public async Task<IActionResult> CategoriesFile([FromForm] FilterDataModel model)
        {
            try
            {
                if (model != null)
                    return Ok(await _storeService.CatergoriesData(model));
                else
                    return BadRequest(new { message = "please at least upload one file " });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Invalid file extension" });
                throw;
            }
        }

        [HttpPost("ImportOrderJobFile")]
        public async Task<IActionResult> OrderJobFile([FromForm] FilterDataModel model)
        {
            try
            {
                if (model != null)
                    return Ok(await _storeService.OrderJobData(model));
                else
                    return BadRequest(new { message = "please at least upload one file " });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Invalid file extension" });
                throw;
            }
        }

        [HttpPost("ImportParametersByDepartmentFile")]
        public async Task<IActionResult> ParametersByDepartmentFile([FromForm] FilterDataModel model)
        {
            try
            {
                if (model != null)
                    return Ok(await _storeService.ParametersByDepartmentData(model));
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

        [HttpPost("GetNewStockData")]
        public async Task<IActionResult> GetNewStockData(FilterDataRequest request)
        {
            try
            {
                return Ok(await _storeService.GetNewStockData(request));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Invalid Id" });
                throw;
            }
        }

        [HttpPost("GetOrderJob")]
        public async Task<IActionResult> GetOrderJob(FilterDataRequest request)
        {
            try
            {
                return Ok(await _storeService.GetOrderJob(request));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Invalid Id" });
                throw;
            }
        }

        [HttpPost("GetReservedData")]
        public async Task<IActionResult> GetReservedData(FilterDataRequest request)
        {
            try
            {
                return Ok(await _storeService.GetReservedData(request));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Invalid Id" });
                throw;
            }
        }

        [HttpPost("GetDepartmentsData")]
        public async Task<IActionResult> GetDepartmentsData(FilterDataRequest request)
        {
            try
            {
                return Ok(await _storeService.GetDepartmentsData(request));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Invalid Id" });
                throw;
            }
        }

        [HttpPost("GetCategoriesData")]
        public async Task<IActionResult> GetCategoriesData(FilterDataRequest request)
        {
            try
            {
                return Ok(await _storeService.GetCategoriesData(request));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Invalid Id" });
                throw;
            }
        }

        [HttpPost("GetParametersByDepartmentData")]
        public async Task<IActionResult> GetParametersByDepartmentData(FilterDataRequest request)
        {
            try
            {
                return Ok(await _storeService.GetParametersByDepartmentData(request));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Invalid Id" });
                throw;
            }
        }


    }
}
