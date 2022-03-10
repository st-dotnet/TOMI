using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TOMI.Services.Interfaces;

namespace TOMI.Web.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class InformationLoadingController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IInfomrationLoadingService _infomrationLoadingService;

        public InformationLoadingController(IMapper mapper,IInfomrationLoadingService infomrationLoadingService )
        {
            _mapper = mapper;
            _infomrationLoadingService = infomrationLoadingService;
        }
        #region
        /// <summary>
        /// GenerateTerminalSummary
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GenerateTerminalSummary")]
        public async Task<IActionResult> GenerateTerminalSummary()
        {
            try
            {
                return Ok(await _infomrationLoadingService.GenerateTerminalSummary());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// GetInformationTransmissionDetails
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetInformationTransmissionDetails")]
        public async Task<IActionResult> GetInformationTransmissionDetails()
        {
            try
            {
                return Ok(await _infomrationLoadingService.GetInformationTransmissionDetails());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// GetInformationforOriginalTag
        /// </summary>
        /// <returns></returns>
        [HttpGet("ForOriginalTag/{tag}")]
        public async Task<IActionResult> ForOriginalTag(int tag)
        {
            try
            {
                return Ok(await _infomrationLoadingService.GetInformationforOriginalTag(tag));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// GetInformationdetails
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetInformationdetails/{tag}/{empnumber}/{terminal}")]
        public async Task<IActionResult> GetInformationdetails(int tag, string empnumber, string terminal)
        {
            try
            {
                return Ok(await _infomrationLoadingService.GetInformationdetails(tag, empnumber, terminal));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        
        #endregion
    }
}
