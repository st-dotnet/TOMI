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
    public class ProgramTerminalController : ControllerBase
    {
        private readonly IProgramTerminalService _programTerminalService;
        private readonly IMapper _mapper;
        private readonly ILogger<ProgramTerminalController> _logger;
        public ProgramTerminalController(ILogger<ProgramTerminalController> logger, IProgramTerminalService programTerminalService, IMapper mapper)
        {
            _programTerminalService = programTerminalService;
            _mapper = mapper;
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into rangeController");
        }

        #region
        /// <summary>
        /// InsertdataMasterFiletoMF1
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GenerateMF1")]
        public async Task<IActionResult> GenerateMF1(TermModel model)
        {
            try
            {
                return Ok(await _programTerminalService.GenerateMF1(model));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// GetMFData
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetMFData")]
        public async Task<IActionResult> GetMFData(GetTerminalModel terminal)
        {
            try
            {
                var response = await _programTerminalService.GetMFData(terminal);
                return Ok(response.Value);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        /// <summary>
        /// PostTerminal
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("PostTerminal")]
        public async Task<IActionResult> PostTerminal(TerminalDataModels post)
        {
            try
            {
                return Ok(await _programTerminalService.PostTerminal(post));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// Empdata
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Empdata")]
        public async Task<IActionResult> Empdata()
        {
            try
            {
                return Ok(await _programTerminalService.AddEmployeeData());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        #endregion
    }
}
