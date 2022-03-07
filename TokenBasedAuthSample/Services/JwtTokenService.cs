using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NETCore.Encrypt;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JWT =  Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TokenBasedAuthSample.Services
{
    // Token oluşturan servisimiz
    public class JwtTokenService : ITokenService
    {
        private IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TokenResult GenerateToken(List<Claim> claims)
        {

            string signingKey = EncryptProvider.AESDecrypt(EncrptedHelper.Replace(_configuration["JWT:signingKey"]), ConfigurationEncryptionTypes.SecretKey, ConfigurationEncryptionTypes.VectorKey);

            string issuer = EncryptProvider.AESDecrypt(EncrptedHelper.Replace(_configuration["JWT:issuer"]), ConfigurationEncryptionTypes.SecretKey, ConfigurationEncryptionTypes.VectorKey);

            string audience = EncryptProvider.AESDecrypt(EncrptedHelper.Replace(_configuration["JWT:audience"]), ConfigurationEncryptionTypes.SecretKey, ConfigurationEncryptionTypes.VectorKey);



            var token = new JwtSecurityToken
               (
                   issuer: issuer, // token yayınlayan kişi payload da taşınacak
                   audience: audience, // token kullanacak olan kişi payload da taşınacak
                   claims: claims, // token içerisinde kullanıcıya ait özgü bilgilerimiz payload kısmında tutacağız
                   expires: DateTime.UtcNow.AddSeconds(3600), // 3600 1 saat boyunca kaynağa erişebiliriz.
                   notBefore: DateTime.UtcNow, // şuan itibari ile token başlat
                   signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                           SecurityAlgorithms.HmacSha512) // private token  // HmacSha512 algoritması ile jwt şifreliyor. Hash algoritması uyguluyor.


               );

            var tokenResponse = new TokenResult
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token), // token base64 formatında dönecektir.
                //RefreshToken = TokenHelper.GetToken()
                ExpireMinutes = 3600
            };


            return tokenResponse;



        }
    }
}
