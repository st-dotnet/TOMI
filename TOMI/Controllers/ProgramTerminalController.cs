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
        [Route("GenerateMasterFiles")]
        public async Task<IActionResult> GenerateMasterFiles(TerminalModel model)
        {
            try
            {
                return Ok(await _programTerminalService.GenerateMasterFiles(model));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        #endregion
    }
}
