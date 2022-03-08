using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NETCore.Encrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TokenBasedAuthSample.Dtos;
using TokenBasedAuthSample.identity;
using TokenBasedAuthSample.Services;

namespace TokenBasedAuthSample.Controllers
{
    

    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase
    {
        ITokenService tokenService1;
        IConfiguration _configuration;
   


        public TokensController(ITokenService tokenService, IConfiguration configuration)
        {
            tokenService1 = tokenService;
            _configuration = configuration;
        }

        [HttpPost("accessToken")] // tokens/accesstoken endpointinden login olup accesstoken alıyoruz.
        public IActionResult Login([FromBody] LoginRequestDto loginRequestDto)
        {
            // "mert".ToUpper(); // MERT


            //string test = EncryptProvider.AESDecrypt(EncrptedHelper.Replace(_configuration["JWT:issuer"]), ConfigurationEncryptionTypes.SecretKey, ConfigurationEncryptionTypes.VectorKey);

            // normalde burada veri tabanından sorguluyoruz
            if(loginRequestDto.Email == "mert.alptekin@neominal.com" && loginRequestDto.Password == "1234")
            {
                // dbden user ile ilgili role ve bilgileri çekip claim'e basabiliriz.
                var claims = new List<Claim>();
                claims.Add(new Claim("name", "mert"));
                claims.Add(new Claim("emailaddress", "mert.alptekin@neominal.com"));
                // not email yerine email address kullanalım dikkat bug var.
                //claims.Add(new Claim(ClaimTypes.Email, "mert.alptekin@neominal.com"));
                claims.Add(new Claim("role", "admin"));

                var response = tokenService1.GenerateToken(claims);


                return Ok(response);

            }

            return Unauthorized(); // 401
           

        }



       
    }
}
