using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;
using TOMI.Services.Interfaces.RangesService;
using TOMI.Services.Models;

namespace TOMI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RangesController : ControllerBase
    {
        private readonly IRangesService _rangeService;
        private readonly IMapper _mapper;
        private readonly ILogger<RangesController> _logger;
        public RangesController(ILogger<RangesController> logger, IRangesService rangeService, IMapper mapper)
        {
            _rangeService = rangeService;
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
        [Route("Addrange")]
        public async Task<IActionResult> AddrangeAsync(RangesModel ranges)
        {
            try
            {
                return Ok(await _rangeService.SaveRanges(ranges));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        ///// <summary>
        ///// Deleterange
        ///// </summary>
        ///// <returns></returns>
        //[HttpDelete("DeleteRange/{rangeId}")]
        //public async Task<IActionResult> DeleteRangeAsync(Guid rangeId)
        //{
        //    try
        //    {
        //        return Ok(await _rangeService.DeleteRange(rangeId));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.ToString());
        //    }
        //}
        /// <summary>
        /// GetrangeList
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetRangeList")]
        public async Task<IActionResult> GetRangeListAsync()
        {
            try
            {
                var response = await _rangeService.GetRangesAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// Getrange
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetRange/{id}")]
        public async Task<IActionResult> GetRangeAsync(Guid id)
        {
            try
            {
                var rangeRequest = _mapper.Map<Ranges>(id);
                var response = await _rangeService.GetRange(rangeRequest.Id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// GetLastRange
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetLastRange/{id}")]
        public async Task<IActionResult> GetLastRange(Guid id)
        {
            try
            {
                var response = await _rangeService.GetLastRange(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        ///  GetMinMaxRange
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMinMaxRange")]
        public async Task<IActionResult> GetMinMaxRange()
        {
            try
            {
                var response = await _rangeService.GetMinMaxRange();
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        #endregion
    }
}
