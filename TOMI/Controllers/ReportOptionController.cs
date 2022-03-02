using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TOMI.Services.Interfaces;
using TOMI.Services.Models;
using TOMI.Services.Repository;

namespace TOMI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportOptionController : ControllerBase
    {
        private readonly IReportOptionService _reportOptionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ReportOptionController> _logger;
        public ReportOptionController(ILogger<ReportOptionController> logger, IReportOptionService reportOptionRepository, IMapper mapper)
        {
            _reportOptionRepository = reportOptionRepository;
            _mapper = mapper;
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into reportOptionController");
        }
        #region Public methods
        /// <summary>
        /// GetLabelDetailsAsync
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(("GetLabelDetailsAsyncTest"))]
        public async Task<IActionResult> GetLabelDetailsAsyncTest()
        {
            try
            {
                var response = await _reportOptionRepository.GetLabelDetailsAsyncTest();
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// GetLabelDetailsAsync
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(("GetLabelDetailsAsync/{tagFrom}/{tagTo}"))]
        public async Task<IActionResult> GetLabelDetailsAsync(int? tagFrom, int? tagTo)
        {
            try
            {
                var response = await _reportOptionRepository.GetLabelDetailsAsync(tagFrom, tagTo);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// GetCodeNotFoundAsync
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Route("GetCodeNotFoundAsync")]
        public async Task<IActionResult> GetCodeNotFoundAsync()
        {

            try
            {
                var response = await _reportOptionRepository.GetCodeNotFoundAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }



        /// <summary>
        /// GetExtendedPricesAsync
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetExtendedPricesAsync")]
        public async Task<IActionResult> GetExtendedPricesAsync()
        {

            try
            {
                var response = await _reportOptionRepository.GetExtendedPricesAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// GetUncountedItemsAsync
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(("GetUncountedItemsAsync"))]
        public IActionResult GetUncountedItemsAsync()
        {
            try
            {
                var response = _reportOptionRepository.GetUncountedItemsAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// GetVariationBySKUAsync
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetVariationBySKUAsync")]
        public IActionResult GetVariationBySKUAsync()
        {
            try
            {
                var response = _reportOptionRepository.GetVariationBySKUAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// GetCorrectionsReportAsync
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetCorrectionsReportAsync")]
        public async Task<IActionResult> GetCorrectionsReportAsync()
        {

            try
            {
                var response = await _reportOptionRepository.GetCorrectionsReportAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// GetBreakDownReportAsync
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBreakDownReportAsync")]
        public IActionResult GetBreakDownReportAsync()
        {
            try
            {
                var response = _reportOptionRepository.GetBreakDownReportAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// GetDateTimeCheckReport
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDateTimeCheckReport")]
        public async Task<IActionResult> GetDateTimeCheckReport()
        {
            try
            {
                var response = await _reportOptionRepository.GetDateTimeCheckReport();
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost("GetInventoryFigureReport")]
        public async Task<IActionResult> GetInventoryFigureReport([FromForm] FilterCustomerReportDataModel model)
        {
            try
            {
                //return Ok(await _reportOptionRepository.InventoryFigure(model));
                
                var invFigure = await _reportOptionRepository.InventoryFigure(model);
                var stream = System.IO.File.OpenRead(invFigure);
                var fileName = System.IO.Path.GetFileName(invFigure);
                return new FileStreamResult(stream, $"application/text|{fileName}");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        [HttpPost("InventoryDetailReport")]
        public async Task<IActionResult> InventoryDetailReport([FromForm] FilterCustomerReportDataModel model)
        {
            try
            {
                //  return Ok(await _reportOptionRepository.InventoryDetail(model));
                var invResult = await _reportOptionRepository.InventoryDetail(model);
                var stream = System.IO.File.OpenRead(invResult);
                var fileName = System.IO.Path.GetFileName(invResult);
                return new FileStreamResult(stream, $"application/text|{fileName}");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost("MarbeteDetailReport")]
        public async Task<IActionResult> MarbeteDetailReport([FromForm] FilterCustomerReportDataModel model)
        {
            try
            {
                // return Ok(await _reportOptionRepository.MarbeteDetail(model));
                var result = await _reportOptionRepository.MarbeteDetail(model);
                var stream = System.IO.File.OpenRead(result);
                var fileName = System.IO.Path.GetFileName(result);
                return new FileStreamResult(stream, $"application/text|{fileName}");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        #endregion Public methods
    }
}
