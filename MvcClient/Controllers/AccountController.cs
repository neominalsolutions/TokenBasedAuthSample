using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Helpers;
using MvcClient.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MvcClient.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient api1;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            api1 = httpClientFactory.CreateClient("api1");
        }

        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("ExternalAuth");

            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginInputModel model)
        {

            var jsonParam = System.Text.Json.JsonSerializer.Serialize(model);

            var param = new StringContent(jsonParam, Encoding.UTF8, "application/json");


            var request = await api1.PostAsync("https://localhost:5001/api/tokens/accessToken", param);

            // apiden token alma isteği

            // apiden token aldıysak
            if (request.IsSuccessStatusCode)
            {
                // cookie oluşturmamız lazım

                var jsonToken = await request.Content.ReadAsStringAsync();

                var tokenModel = System.Text.Json.JsonSerializer.Deserialize<TokenModel>(jsonToken, JsonOptionsHelper.GetOptions());

                // access token parse ettik.
                // System.IdentityModel.Tokens.Jwt bu kütüphaneyi kullandık.
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(tokenModel.AccessToken);

                //jwtToken.pa




                int expireDateSeconds = (int)jwtToken.Payload.Exp;
                // user hesabına ait hesap için geçerli claim bilgileri saklanır.
                var claimPrinciple = new ClaimsPrincipal();

                var identity = new ClaimsIdentity(jwtToken.Payload.Claims, "ExternalAuth");
                claimPrinciple.AddIdentity(identity);

                // kimlik doğrulanırken saklanacak olan değerler özellikler
                // burada token, token süresi, token kalıcı olup olmaycağı gibi bilgiler saklarnır.
                var authProperties = new AuthenticationProperties();
                authProperties.IsPersistent = true;
                authProperties.ExpiresUtc = DateTimeOffset.FromUnixTimeSeconds(expireDateSeconds);


                // authentication Token oluştur.
                var accessToken = new AuthenticationToken();
                accessToken.Name = "AccessToken";
                accessToken.Value = tokenModel.AccessToken;

                // sisteme accesstoken AuthenticationToken olarak ekledik
                var tokens = new List<AuthenticationToken>();
                tokens.Add(accessToken);

                // uygulamada HttpContext üzerinde access ve refresh token saklayacağız ki httpcontext üzerinden bu token bilgilerine ulaşıp request atarken request header'a bu bilgileri gömeceğiz.

                authProperties.StoreTokens(tokens);
                // httpcontext üzerinde token sakladığımız method. Inmemory olarak saklar. Uygulama çalıştığı sürece saklanır. session bazlı tutar.


                await HttpContext.SignInAsync("ExternalAuth", claimPrinciple, authProperties);


                return Redirect("/");

            }

            ViewBag.Message = "Kullanıcı e-posta yada parola yanlış";

            return View();
        }
    }
}
