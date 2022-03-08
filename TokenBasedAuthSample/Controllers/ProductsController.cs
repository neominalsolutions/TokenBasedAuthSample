using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokenBasedAuthSample.Dtos;

namespace TokenBasedAuthSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // hangi sheme için authorize
  
    // Scheme Default ismini Bearer koymuşlar.
    
    public class ProductsController : ControllerBase
    {
        
        public List<ProductDto> productDtos
        {
            get
            {
                return new List<ProductDto>() {
                    new ProductDto
                    {
                        Name = "P1",
                        Price = 10,
                        Stock = 10
                    },
                    new ProductDto
                    {
                        Name = "p2",
                        Price = 20,
                        Stock = 30
                    }
                };
            }
        }

        [HttpGet] // api/products
        [Authorize(AuthenticationSchemes = "myScheme")]
        public IActionResult GetProducts()
        {
            return Ok(productDtos);
        }


        [Authorize(Roles ="admin", AuthenticationSchemes = "myScheme")]
        // claim olarak role claim bakar. özel bir durum.
        [HttpGet("AdminEndPoint")]
        public IActionResult AdminEndPoint()
        {
            return Ok("Admin Access");
        }

        [Authorize("emailRequired", AuthenticationSchemes = "myScheme")]
        [HttpGet("ClaimAccessEndPoint")]
        public IActionResult ClaimAccessEndPoint()
        {
            return Ok("EmailClaimAccess");
        }

        [Authorize(Policy = "SpesificEmailAddress", AuthenticationSchemes = "myScheme")]
        [HttpGet("SpesificEmailAddress")]
        public IActionResult EmailDomainEndsWithNeominal()
        {
            return Ok("SpesificEmailAddressAccess");
        }


        [Authorize(Policy = "AgeRequired", AuthenticationSchemes = "myScheme")]
        [HttpGet("AgeRequired")]

        // access token var yetki yetersiz ise 403 forbiden alırız.
        public IActionResult AgeRequired()
        {
            return Ok("AgeRequiredAccess");
        }

        //[Authorize(AuthenticationSchemes = "google")]


    }
}
