using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;
using TOMI.Services.Interfaces;
using TOMI.Services.Models;

namespace TOMI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockAdjustmentController : ControllerBase
    {
        private readonly IStockAdjustmentService _stockAdjustmentService;
        private readonly IMapper _mapper;
        private readonly ILogger<StockAdjustmentController> _logger;
        public StockAdjustmentController(ILogger<StockAdjustmentController> logger, IStockAdjustmentService stockAdjustmentService, IMapper mapper)
        {
            _stockAdjustmentService = stockAdjustmentService;
            _mapper = mapper;
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into rangeController");
        }

        #region Public methods

        /// <summary>
        /// Addrange
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AddStockAdjustment")]
        public async Task<IActionResult> AddStockAdjustment(StockAdjustmentModel model)
        {
            try
            {
                return Ok(await _stockAdjustmentService.SaveStockAdjustment(model));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// DeleteStockAdjustment
        /// </summary>
        /// <returns></returns>
        [HttpDelete("DeleteStockAdjustment/{Id}")]
        public async Task<IActionResult> DeleteStockAdjustment(Guid Id)
        {
            try
            {
                return Ok(await _stockAdjustmentService.DeleteStockAdjustment(Id));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// GetStockAdjustmentListAsync
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetStockAdjustmentListAsync")]
        public async Task<IActionResult> GetStockAdjustmentListAsync()
        {
            try
            {
                var response = await _stockAdjustmentService.GetStockAdjustmentListAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// GetStockAdjustmentAsync
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetStockAdjustmentAsync/{id}")]
        public async Task<IActionResult> GetStockAdjustmentAsync(Guid id)
        {
            try
            {
                var response = await _stockAdjustmentService.GetStockAdjustmentAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// GoToRecord
        /// </summary>
        /// <returns></returns>
        [HttpGet("GoToRecord/{recId}")]
        public async Task<IActionResult> GoToRecord(int recId)
        {
            try
            {
                var response = await _stockAdjustmentService.GoToRecord(recId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// GetDeletedRecord
        /// </summary>
        /// <returns></returns> 
        [HttpGet("GetDeletedRecord")]
        public async Task<IActionResult> GetDeletedRecord()
        {
            try
            {
                var response = await _stockAdjustmentService.GetDeletedRecord();
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// ChangeDeletedRecStatus
        /// </summary>
        /// <returns></returns>
        [HttpGet("ChangeDeletedRecStatus/{recid}")]
        public async Task<IActionResult> ChangeDeletedRecStatus(Guid recid)
        {
            try
            {
                var response = await _stockAdjustmentService.ChangeDeletedRecStatus(recid);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// MasterData
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        [HttpGet("MasterDataBySku/{sku}")]
        public async Task<IActionResult> MasterData(string sku)
        {
            try
            {
                var response = await _stockAdjustmentService.MasterDataBySku(sku);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost]
        [Route("SearchStockAdjustment")]
        public async Task<IActionResult> SearchStockAdjustment(StockAdjustmentFilterModel model)
        {
            try
            {
                return Ok(await _stockAdjustmentService.FilterStock(model));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpGet]
        [Route("GetMasterByCustomerId/{custid}")]
        public async Task<IActionResult> GetMasterByCustomerId(Guid custid)
        {
            try
            {
                return Ok(await _stockAdjustmentService.GetMasterDataByCustomerId(custid));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            #endregion
        }
    }
}
