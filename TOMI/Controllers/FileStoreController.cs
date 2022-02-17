using AutoMapper;
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
    public class FileStoreController : ControllerBase
    {
        private readonly IFileStoreService _fileStoreService;
        private readonly IMapper _mapper;
        private readonly ILogger<FileStoreController> _logger;
        public FileStoreController(ILogger<FileStoreController> logger, IFileStoreService fileStoreService, IMapper mapper)
        {
            _fileStoreService = fileStoreService;
            _mapper = mapper;
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into CustomerController");
        }
        #region Public methods
        /// <summary>
        /// GetInfoFileLoadedList
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetInfoFileLoadedListAsync")]
        public async Task<IActionResult> GetInfoFileLoadedListAsync(FileStoreModel model)
        {
            try
            {
                var response = await _fileStoreService.GetFileStoreAsync(model);
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
