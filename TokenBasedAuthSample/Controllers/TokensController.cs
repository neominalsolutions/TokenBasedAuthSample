using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NETCore.Encrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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

        [HttpGet]
        public IActionResult Test()
        {
            // "mert".ToUpper(); // MERT


            string test = EncryptProvider.AESDecrypt(EncrptedHelper.Replace(_configuration["JWT:issuer"]), ConfigurationEncryptionTypes.SecretKey, ConfigurationEncryptionTypes.VectorKey);


            var claims = new List<Claim>();
            claims.Add(new Claim("name", "muhammed"));
            claims.Add(new Claim("role", "developer"));

            var response =  tokenService1.GenerateToken(claims);


            return Ok(response);


        }
    }
}
