using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TokenBasedAuthSample.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        // google yada facebook bağlanır. oraya yönlenir. facebook yada google üzerinden login olmalıyız. Güvenli olması açısından.
        public IActionResult ExternalLogin(string providerName, string returnUrl = null)
        {

            // yani ilgili provider ait sign-in sayfasına yönlendirir bizi.
            // sign-in callback yazmamızın sebebi buradan changeresult class kullanrak gelen isteği başka bir yere callback etmektir.
            return new ChallengeResult(
             providerName,
             new AuthenticationProperties
             {
                 RedirectUri = Url.Action(nameof(ExternalLoginCallback))
             });
        }

        [HttpGet]
        [AllowAnonymous]
        // google arayüzünden login olduktan sonra bu method tetiklenecek.
        public async Task<IActionResult> ExternalLoginCallback()
        {
            // böyle googledan login olduktan sonra aldığımız bilgiler ile External scheme üzerinden sisteme login olduk
            var authenticateResult = await HttpContext.AuthenticateAsync("External"); // External Scheme ile login ol
            // dış kaynaktaki accesstoken bişlgilerine erişlip kimlik doğrulanıyor.

            var token = await HttpContext.GetTokenAsync("External","access_token");
            // bu şemadan gelen accessToken bilgisi ClaimsPrinciple class HttpContext.AuthenticateAsync methodu ile doldurulur.


            // authenticated isek
            if (!authenticateResult.Succeeded)
                return BadRequest(); // TODO: Handle this better.

            // kendi şemamıza google şemasına
            var claimsIdentity = new ClaimsIdentity("google");

            claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier));
            claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.Email));

            // burada şemaya cookie oluşuyor
            await HttpContext.SignInAsync(
                "google",
                new ClaimsPrincipal(claimsIdentity));

            return View();


        }

        public async Task<IActionResult> ExternalLogOut()
        {
            await HttpContext.SignOutAsync("google"); // kendi uygulmamdaki cookie temizler
            await HttpContext.SignOutAsync("External"); // google cookiesini temizler

            return Redirect("/");
        }
    }
}
