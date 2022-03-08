using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TokenBasedAuthSample.Dtos
{
    // System.Text.Json paketi ile json dönecek olan class üzerinde işlem yapabiliriz.
   
   
    public class ProductDto
    {
        
        [JsonPropertyName("productName")]
        public string Name { get; set; }

        [JsonPropertyName("unitPrice")]
      
        public decimal Price { get; set; }

        [JsonPropertyName("unitsInStock")]
        
        public int Stock { get; set; }

    }
}
