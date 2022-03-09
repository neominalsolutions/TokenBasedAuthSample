using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Helpers;
using MvcClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MvcClient.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient api1;

        public ProductController(IHttpClientFactory httpClientFactory)
        {
            api1 = httpClientFactory.CreateClient("api1");
        }

        public async Task<IActionResult> Public()
        {

            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri("https://localhost:5001");

            //    client.GetAsync();
            //}



            //api1.SetBearerToken("dsadasd");

            var result = await api1.GetAsync("https://localhost:5001/api/products/public");

            

            if (result.IsSuccessStatusCode) // 200 alırsak
            {
                var jsonString = await result.Content.ReadAsStringAsync(); // request body json string olarak okur.

                var model =  System.Text.Json.JsonSerializer.Deserialize<List<ProductViewModel>>(jsonString,JsonOptionsHelper.GetOptions());

                return View(model);

            }


            return NotFound();
        }


        public async Task<IActionResult> Protected2()
        {
           
             
                var result = await api1.GetAsync("https://localhost:5001/api/products/protected");


                if (result.IsSuccessStatusCode) // 200 alırsak
                {
                    var jsonString = await result.Content.ReadAsStringAsync(); // request body json string olarak okur.

                    var model = System.Text.Json.JsonSerializer.Deserialize<List<ProductViewModel>>(jsonString, JsonOptionsHelper.GetOptions());

                    return View(model);

                }

 
            return Unauthorized();
        }

        public async Task<IActionResult> Protected()
        {
            var jsonParam = System.Text.Json.JsonSerializer.Serialize(new LoginInputModel { Email = "mert.alptekin@neominal.com", Password = "1234" });

            var param = new StringContent(jsonParam, Encoding.UTF8, "application/json");


           var request =  await api1.PostAsync("https://localhost:5001/api/tokens/accessToken", param);

            // apiden token alma isteği

            // apiden token aldıysak
           if(request.IsSuccessStatusCode)
            {
                var jsonToken = await request.Content.ReadAsStringAsync();
                // token response jsonString çevir.

                var tokenModel = System.Text.Json.JsonSerializer.Deserialize<TokenModel> (jsonToken, JsonOptionsHelper.GetOptions());
                // token  model class dönüştür.

                // bearer token olarak headers authorization olarak ekle
             api1.SetBearerToken(tokenModel.AccessToken);
            var result = await api1.GetAsync("https://localhost:5001/api/products/protected");


                if (result.IsSuccessStatusCode) // 200 alırsak
                {
                    var jsonString = await result.Content.ReadAsStringAsync(); // request body json string olarak okur.

                    var model = System.Text.Json.JsonSerializer.Deserialize<List<ProductViewModel>>(jsonString, JsonOptionsHelper.GetOptions());

                    return View(model);

                }

            }


            return Unauthorized();
        }
    }
}
