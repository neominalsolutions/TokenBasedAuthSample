using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MvcClient.Helpers
{
    public static class JsonOptionsHelper
    {

        public static JsonSerializerOptions GetOptions()
        {
            return new JsonSerializerOptions
            {

                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,


            };
        }
    }
}
