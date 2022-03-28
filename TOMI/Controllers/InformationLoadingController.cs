using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TOMI.Services.Interfaces;
using TOMI.Services.Models;

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
        /// GetInformationfirstSectiondetails
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetInformationfirstSectiondetails/{tag}/{empNumber}")]
        public async Task<IActionResult> GetInformationfirstSectiondetails(string tag,string empNumber)
        {
            try
            {
                return Ok(await _infomrationLoadingService.GetInformationfirstsectiondetails(tag, empNumber));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// GetInformationsecondSectiondetails
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetInformationsecondSectiondetails/{tag}/{empNumber}")]
        public async Task<IActionResult> GetInformationsecondSectiondetails(string tag, string empNumber)
        {
            try
            {
                return Ok(await _infomrationLoadingService.GetInformationsecondsectiondetails(tag, empNumber));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// DeleteOriginalRecord
        /// </summary>
        /// <returns></returns>
        [HttpDelete("DeleteOriginalRecord/{tag}/{empNumber}/{terminal}")]
        public async Task<IActionResult> DeleteOriginalRecord(int tag,string empNumber,string terminal)
        {
            try
            {
                return Ok(await _infomrationLoadingService.DeleteOriginalRecord(tag,empNumber,terminal));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// UpdateTag
        /// </summary>
        /// <returns></returns>
        [HttpPost("UpdateTag")]
        public async Task<IActionResult> UpdateTag(UpdateTag updateTag)
        {
            try
            {
                return Ok(await _infomrationLoadingService.ReNumberTagOption(updateTag));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// MergeNewRecordWithOriginal
        /// </summary>
        /// <returns></returns>
        [HttpPost("MergeNewRecordWithOriginal")]
        public async Task<IActionResult> MergeNewRecordWithOriginal(MergeWithNewInfloarding mergeWithNewInfloarding)
        {
            try
            {
                return Ok(await _infomrationLoadingService.MergeNewithOriginal(mergeWithNewInfloarding));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        #endregion
    }
}
