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
    public class DwnErrorsController : ControllerBase
    {
        private readonly IDwnErrorsService _dwnErrorsService;
        private readonly IMapper _mapper;
        private readonly ILogger<DwnErrorsController> _logger;
        public DwnErrorsController(ILogger<DwnErrorsController> logger, IDwnErrorsService dwnErrorsService, IMapper mapper)
        {
            _dwnErrorsService = dwnErrorsService;
            _mapper = mapper;
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into rangeController");
        }
        #region Public Methods
        /// <summary>
        /// AddDwnErrors
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddDwnErrors")]
        public async Task<IActionResult> AddDwnErrors(DwnErrorsModel model)
        {
            try
            {
                return Ok(await _dwnErrorsService.SaveDwnErrors(model));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// DeleteDwnErrors
        /// </summary>
        /// <returns></returns>
        [HttpDelete("DeleteDwnErrors/{Id}")]
        public async Task<IActionResult> DeleteDwnErrors(int Id)
        {
            try
            {
                return Ok(await _dwnErrorsService.DeleteDwnErrors(Id));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// GetDwnErrorsAsyncById
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDwnErrorsAsync/{id}")]
        public async Task<IActionResult> GetDwnErrorsAsync(int id)
        {
            try
            {
                var response = await _dwnErrorsService.GetDwnErrorsAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// GetDwnErrorsListAsync
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDwnErrorsListAsync")]
        public async Task<IActionResult> GetDwnErrorsListAsync()
        {
            try
            {
                var response = await _dwnErrorsService.GetDwnErrorsListAsync();
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
