using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
        public UserController( IUserService userService,IMapper mapper)
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
    }
}
