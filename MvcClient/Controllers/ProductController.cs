using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Helpers;
using MvcClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        public async Task<IActionResult> Index()
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
    }
}
