using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    public class InfoLoadController : ControllerBase
    {
        private readonly IInfoLoadService _infoLoadService;
        private readonly IMapper _mapper;
        private readonly ILogger<InfoLoadController> _logger;
        public InfoLoadController(ILogger<InfoLoadController> logger, IInfoLoadService infoLoadService, IMapper mapper)
        {
            _infoLoadService = infoLoadService;
            _mapper = mapper;
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into rangeController");
        }

        #region Public Methods
        /// <summary>
        /// AddInfoLoad
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddInfoLoad")]
        public async Task<IActionResult> AddInfoLoad(InfoLoadModel model)
        {
            try
            {
                return Ok(await _infoLoadService.SaveInfoLoad(model));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// DeleteInfoLoad
        /// </summary>
        /// <returns></returns>
        [HttpDelete("DeleteInfoLoad/{Id}")]
        public async Task<IActionResult> DeleteInfoLoad(int Id)
        {
            try
            {
                return Ok(await _infoLoadService.DeleteInfoLoad(Id));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// GetInfoLoadAsyncById
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetInfoLoadAsync/{Id}")]
        public async Task<IActionResult> GetInfoLoadAsync(int id)
        {
            try
            {
                var response = await _infoLoadService.GetInfoLoadAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// GetInfoLoadListAsync
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetInfoLoadListAsync")]
        public async Task<IActionResult> GetInfoLoadListAsync()
        {
            try
            {
                var response = await _infoLoadService.GetInfoLoadListAsync();
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
