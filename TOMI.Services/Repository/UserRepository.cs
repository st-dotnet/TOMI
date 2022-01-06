using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TOMI.Data.Database;
using TOMI.Data.Database.Entities;
using TOMI.Services.Interfaces;

namespace TOMI.Services.Repository
{

   public class UserRepository: IUserService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UserRepository> _logger;
        private readonly TOMIDataContext _context;
        public UserRepository(ILogger<UserRepository> logger, TOMIDataContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public User Authenticate(string username, string password)
        {
            try
            {
                User user = new User();
               var passwordHasher = new PasswordHasher<User>();
                user = _context.Users.FirstOrDefault(x => x.Email == username);
                if (user != null)
                {
                    var result =  passwordHasher.VerifyHashedPassword(user,user.Password, "Sss1234!");
                    if (result == PasswordVerificationResult.Success)
                        return user;
                    else
                        return null;
                }

                return user;
            }
            catch (Exception ex)
            { 
                throw;
            }
            
        }
    }
}
