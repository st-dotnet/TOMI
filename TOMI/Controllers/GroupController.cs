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
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IMapper _mapper;
        private readonly ILogger<GroupController> _logger;
        public GroupController(ILogger<GroupController> logger, IGroupService groupService, IMapper mapper)
        {
            _groupService = groupService;
            _mapper = mapper;
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into rangeController");
        }

        #region Public methods

        /// <summary>
        /// AddGroup
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AddGroup")]
        public async Task<IActionResult> AddGroupAsync(GroupModel model)
        {
            try
            {
                return Ok(await _groupService.AddGroup(model));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        /// <summary>
        /// DeleteGroup
        /// </summary>
        /// <returns></returns>
        [HttpDelete("DeleteGroup/{id}")]
        public async Task<IActionResult> DeleteGroup(Guid id)
        {
            try
            {
                return Ok(await _groupService.DeleteGroup(id));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// GetGroupList
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetGroupList")]
        public async Task<IActionResult> GetGroupListAsync()
        {
            try
            {
                var response = await _groupService.GetGroupAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// GetGroup
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetGroup/{id}")]
        public async Task<IActionResult> GetGroup(Guid id)
        {
            try
            {
                // var groupRequest = _mapper.Map<Group>(id);
                var response = await _groupService.GetGroup(id);
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
