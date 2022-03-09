using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcClient.Models
{
    public class TokenModel
    {

        public string AccessToken { get; set; } 
        public int ExpireMinutes { get; set; } 
        public string RefreshToken { get; set; } 
    }


}
