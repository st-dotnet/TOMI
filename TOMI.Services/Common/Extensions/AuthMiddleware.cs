using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TOMI.Data.Database.Entities;
using TOMI.Services.Common.Setting;

namespace TOMI.Services.Common.Extensions
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptions<ConfigSettings> _configSettings;

        public AuthMiddleware(RequestDelegate next, IOptions<ConfigSettings> configSettings)
        {
            _next = next;
            _configSettings = configSettings;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                AttachUserToContext(context, token);
            await _next(context);
        }
        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configSettings.Value.JwtSettings.Key);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var customer = new Customer
                {
                   
                    Name = jwtToken.Claims.First(x => x.Type == "Name").Value,
                 
                };
                // attach user to context on successful jwt validation
                context.Items["Customer"] = customer;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}