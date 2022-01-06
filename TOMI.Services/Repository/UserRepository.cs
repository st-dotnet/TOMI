using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TOMI.Data.Database;
using TOMI.Data.Database.Entities;
using TOMI.Services.Helpers;
using TOMI.Services.Interfaces;
using TOMI.Services.Models;

namespace TOMI.Services.Repository
{

    public class UserRepository : IUserService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UserRepository> _logger;
        private readonly TOMIDataContext _context;
        private readonly AppSettings _appSettings;
        public UserRepository(ILogger<UserRepository> logger, TOMIDataContext context, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        public UserModelResponse Authenticate(string email, string password)
        {
            try
            {
                User user = _context.Users.FirstOrDefault(x => x.Email == email);
                if (user != null)
                {
                    // for verify the hash password
                    var passwordHasher = new PasswordHasher<User>();
                    var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);
                    if (result == PasswordVerificationResult.Success)
                    {
                        var token = GenerateJwtToken(user, user.Email);
                        return new UserModelResponse
                        {
                            User = user,
                            Token = token, 
                            Success= true
                        };
                    }
                }
                return new UserModelResponse { Error = "Invalid UserName and Password " };
            }
            catch (Exception ex)
            {
                return new UserModelResponse { Error = "Invalid UserName and Password " };
            }

        }


        //Generate JWT token
        private string GenerateJwtToken(User customer, string email)
        {
            try
            {
                // generate token that is valid for 7 days
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                    new Claim("customerId", customer.Id.ToString()),
                    new Claim("firstName", customer.FirstName),
                    new Claim("lastName", customer.LastName),
                    new Claim("email", email)

                }),
                    Expires = DateTime.UtcNow.AddDays(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };



                var token = tokenHandler.CreateToken(tokenDescriptor);



                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

    }
}
