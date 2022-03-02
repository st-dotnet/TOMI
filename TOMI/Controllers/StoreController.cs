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
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
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
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
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
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
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
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
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
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
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
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
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
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost("GetOrderJob")]
        public async Task<IActionResult> GetOrderJob(FilterDataRequest request)
        {
            try
            {
                return Ok(await _storeService.GetOrderJob(request));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost("GetReservedData")]
        public async Task<IActionResult> GetReservedData(FilterDataRequest request)
        {
            try
            {
                return Ok(await _storeService.GetReservedData(request));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost("GetDepartmentsData")]
        public async Task<IActionResult> GetDepartmentsData(FilterDataRequest request)
        {
            try
            {
                return Ok(await _storeService.GetDepartmentsData(request));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost("GetCategoriesData")]
        public async Task<IActionResult> GetCategoriesData(FilterDataRequest request)
        {
            try
            {
                return Ok(await _storeService.GetCategoriesData(request));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost("GetParametersByDepartmentData")]
        public async Task<IActionResult> GetParametersByDepartmentData(FilterDataRequest request)
        {
            try
            {
                return Ok(await _storeService.GetParametersByDepartmentData(request));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
