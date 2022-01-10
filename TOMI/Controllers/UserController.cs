using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;
using TOMI.Services.Interfaces;
using TOMI.Services.Models;

namespace TOMI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {

        private IUserService _userService;
        private IMapper _mapper;
        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(UserModel users)
        {
            if (users.Email == null || users.Password == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            var user = _mapper.Map<User>(users);
            // return basic user info and authentication token
            return Ok(_userService.Authenticate(user.Email, user.Password));
        }

        /// <summary>
        /// AddCustomer
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AddUser")]
        public async Task<IActionResult> AddCustomerAsync(User user)
        {
            try
            {
                return Ok(await _userService.CreateUser(user));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        /// <summary>
        /// AddCustomer
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AddUserStore")]
        public async Task<IActionResult> AddCustomerStoreAsync(User user)
        {
            try
            {
                return Ok(await _userService.CreateCustomerStore(user));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
