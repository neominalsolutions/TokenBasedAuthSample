using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TokenBasedAuthSample.Services
{
    public class TokenResult
    {
        public string AccessToken { get; set; } // accesstoken kendisini saklayacağız
        public int ExpireMinutes { get; set; } // Ne kadar süre sonra expire olacak
        public string RefreshToken { get; set; } // AccessToken Yenilemek için kullanacağız

        //public TokenResult()
        //{
        //    new Claim("role", "admin");
        //    new Claim("email", "razor@test.com");

        //}
       
    }

    // List<Claim> nesnesi authenticate olacak kullanıclara ait claimlerin accesstoken içersinde payload olarak saklanamasını sağlayacak.
    public interface ITokenService
    {
        TokenResult GenerateToken(List<Claim> claims);
    }
}
